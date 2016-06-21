﻿var net = require('net');  
var chatServer = net.createServer()
var receiveNick = false;
var clientList = [];
var clientNick = '';
 
chatServer.on('connection', function(client) 
{  
    console.log('welcome ' +  client.localAddress + ':'+ client.remotePort);
    clientList.push(client);
	receiveNick = false;
  
    client.on('data', function(data) {
		if(!receiveNick)
		{
			clientNick = data;
			broadcast(clientNick + ' has joined the chat');
			receiveNick = true;
		}
		else
		{
			broadcast(clientNick + '>' + data);
		}
    });  
	
	client.on('end', function() 
	{  
		clientList.splice(clientList.indexOf(client), 1);
	}) 
	
    client.on('error', function(e) 
	{  
		console.log(e);  
	});
});  

function broadcast(message) 
{  
	var cleanup = []
    for(var i=0;i<clientList.length;i+=1) {      
		if(clientList[i].writable) 
		{
			clientList[i].write(message + '\n')
			console.log(message);
		} else 
		{  
			cleanup.push(clientList[i]) 
			clientList[i].destroy()  
		}      
    }
  	for(i=0;i<cleanup.length;i+=1) 
	{  
		clientList.splice(clientList.indexOf(cleanup[i]), 1)
	}
}  

chatServer.listen(5819,'192.168.16.150', function() 
{ 
	console.log('server 9000 on on on');
});