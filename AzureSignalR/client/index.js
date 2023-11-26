function ajax(method, url, cb) {
  var xmlhttp = new XMLHttpRequest();

  xmlhttp.onreadystatechange = function () {
    if (xmlhttp.readyState == XMLHttpRequest.DONE) {
      if (xmlhttp.status == 200) {
        return cb(null, JSON.parse(xmlhttp.responseText));
      }
      cb(xmlhttp.status + " error");
    }
  };

  xmlhttp.open(method, url, true);
  xmlhttp.send();
}

function addtext(message, cls) {
  var li = document.createElement("li");
  var text = document.createTextNode(message);
  li.appendChild(text);
  li.classList = [cls];
  document.getElementById("messages").appendChild(li);
}

function bindConnectionMessage(connection) {
  var messageCallback = function (name, message) {
    if (!message) return;
    addtext(message, "system");
  };  
  connection.on("echo", messageCallback);
}


var connection;


function connect(url, token) {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(url, { accessTokenFactory: () => token })
    .build();

  bindConnectionMessage(connection);
  connection
    .start()
    .catch(function (error) {
      console.error(error.message);
    });
}

ajax("GET", "http://localhost:5065/login", function (err, data) {
  connect(data.url, data.accessToken);
});

function init() {
  document.getElementById("btn").addEventListener("click", async (e) => {
    try {
      var message = document.getElementById("text").value;
      addtext(message, "user");
      document.getElementById("text").value = "";
      await connection.invoke("echo", "user", message);
    } catch (err) {
      console.error(err);
    }
  });
}
