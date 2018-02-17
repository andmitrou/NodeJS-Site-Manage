const port = require("./nsm-server-config");
const express = require('express')
const app = express()

let helloMessage = 'Starter app listening on port ' + port.portNumber;

app.get('/', function (req, res) {
    
    res.send(helloMessage)
})

app.listen(port.portNumber, function () {
    console.log(helloMessage)
})