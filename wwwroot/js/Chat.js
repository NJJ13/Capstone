"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, timeSent,) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg + " at " + timeSent;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    console.log("Hello")
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var eventId = document.getElementById("eventId").value;
    var currentDate = new Date();
    var dateString = (currentDate.getMonth() + 1) + '/' + currentDate.getDate() + '/' + currentDate.getFullYear() + " " + currentDate.getHours() + ":" + currentDate.getMinutes() + ":" + currentDate.getSeconds();
    var fullMessage = user + " says " + message + " at " + dateString;
    $.ajax({
        url: 'https://localhost:44398/Athlete/SaveMessage',
        dataType: 'json',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ EventId: parseInt(eventId), Message: fullMessage }),
        success: function () {
            console.log("Complete")
        },
        error: function (errorThrown) {
            console.log(errorThrown);
        }
    });
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});