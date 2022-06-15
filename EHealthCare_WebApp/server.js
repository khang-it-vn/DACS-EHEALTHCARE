const express = require("express");
const path = require("path");
var app = express();
//const {conn,sql} = require("./dbconnect");
var server = app.listen(3000, function () {
    console.log("Listening on port 3000");
});
const fs = require("fs");
// const fileUpload = require("express-fileupload");
const io = require("socket.io")(server, {
    allowEIO3: true, // false by default
});
//
// var detail_rooms = [];
// const sql = require("mssql");
// const PORT = process.env.PORT || 8080
//
//
//   // config DB
//   var config = {
//     user: 'sa',
//     password: 'abc@123',
//     server: 'localhost',
//     database: 'demoverse',
//     options: {
//       trustedConnection: true,
//       encrypt: true,
//       enableArithAbort: true,
//       trustServerCertificate: true,
//     }
//   };
// var resulSet;
//   // connect database
// async function getUsers() {
//   try {
//     let pool = await sql.connect(config);
//     let users = await pool.request().query("SELECT d.EMAIL AS EMAIL, d.KEY_ROOM AS KEY_ROOM from DETAIL_ROOM d");
//     return users.recordset;
//   }
//   catch (error) {
//     console.log(error);
//   }
// }
// resulSet = getUsers();
// resulSet.then(function (){
//   console.log( resulSet);
// })



// sql.connect(config, function (err) {
//
//   if (err){
//     console.log(err);
//   }
//
//   // create Request object
//   var request = new sql.Request();
//
//   // query to the DB
//   request.query('SELECT * FROM DETAIL_ROOM', function (err, recordDataSet) {
//
//     if (err){
//       console.log(err);
//     }
//     resulSet = recordDataSet.recordsets;
//     console.log( recordDataSet);
//   });
//   console.log(resulSet);
// });

//
app.use(express.static(path.join(__dirname, "")));
var userConnections = [];
io.on("connection", (socket) => {
    console.log("socket id is ", socket.id);
    socket.on("userconnect", (data) => {
        console.log("userconnect", data.displayName,data.uid, data.meetingid);
        var other_users = userConnections.filter(
          (p) => p.meeting_id == data.meetingid
        );
        userConnections.push({
            connectionId: socket.id,
            user_id: data.uid,
            user_name: data.displayName,
            meeting_id: data.meetingid,
        });
        var userCount = userConnections.length;
        console.log(userCount);
        other_users.forEach((v) => {
            socket.to(v.connectionId).emit("inform_others_about_me", {
                other_user_id: data.uid,
                user_name: data.displayName,
                connId: socket.id,
                userNumber: userCount,
            });
        });
        socket.emit("inform_me_about_other_user", other_users);
    });
    socket.on("SDPProcess", (data) => {
        socket.to(data.to_connid).emit("SDPProcess", {
            message: data.message,
            from_connid: socket.id,
        });
    });
    socket.on("sendMessage", (msg) => {
        console.log(msg);
        var mUser = userConnections.find((p) => p.connectionId == socket.id);
        if (mUser) {
            var meetingid = mUser.meeting_id;
            var from = mUser.user_name;
            var list = userConnections.filter((p) => p.meeting_id == meetingid);
            list.forEach((v) => {
                socket.to(v.connectionId).emit("showChatMessage", {
                    from: from,
                    message: msg,
                });
            });
        }
    });
    socket.on("fileTransferToOther", (msg) => {
        console.log(msg);
        var mUser = userConnections.find((p) => p.connectionId == socket.id);
        if (mUser) {
            var meetingid = mUser.meeting_id;
            var from = mUser.username;
            var list = userConnections.filter((p) => p.meeting_id == meetingid);
            list.forEach((v) => {
                socket.to(v.connectionId).emit("showFileMessage", {
                    username: msg.username,
                    meetingid: msg.meetingid,
                    filePath: msg.filePath,
                    fileName: msg.fileName,
                });
            });
        }
    });
    // delete db user
    socket.on("disconnect",  function () {
        var disUser = userConnections.find((p) => p.connectionId == socket.id);
        console.log("Disconnected user socket id is " + socket.id);
        //var pool =  await conn;
        //var sqlString = `delete DETAIL_ROOM where email = '${disUser.user_id}'  and KEY_ROOM = ${disUser.meeting_id}`
        //await pool.request().query(sqlString,function (err,data){
        //    console.log(data);
        //})

        if (disUser) {
            var meetingid = disUser.meeting_id;
            userConnections = userConnections.filter(
              (p) => p.connectionId != socket.id
            );
            var list = userConnections.filter((p) => p.meeting_id == meetingid);
            list.forEach((v) => {
                var userNumberAfUserLeave = userConnections.length;
                socket.to(v.connectionId).emit("inform_other_about_disconnected_user", {
                    connId: socket.id,
                    uNumber: userNumberAfUserLeave,
                });
            });
        }
    });
});

// app.use(fileUpload());
app.post("/attachimg", function (req, res) {
    var data = req.body;
    var imageFile = req.files.zipfile;
    console.log(imageFile);
    var dir = "public/attachment/" + data.meeting_id + "/";
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    imageFile.mv(
      "public/attachment/" + data.meeting_id + "/" + imageFile.name,
      function (error) {
          if (error) {
              console.log("couldn't upload the image file , error: ", error);
          } else {
              console.log("Image file successfully uploaded");
          }
      }
    );
});
