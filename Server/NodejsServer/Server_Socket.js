var net = require('net');  
var chatServer = net.createServer()
var clientList = [];

chatServer.on('connection', function(client) 
{  
    console.log(client.localAddress + ':'+ client.remotePort +  ' is joined');
    clientList.push(client);
	var clientNick = '';
	var receiveNick = false;
  
    client.on('data', function(data) {
		if(!receiveNick)
		{
			clientNick = data;
			broadcast(clientNick + ' has joined the chat');
			console.log(clientNick + ' has joined the chat');
			receiveNick = true;
		}
		else
		{
			broadcast(clientNick + '>' + data);
			console.log(clientNick + '>' + data);
		}
    });  
	
	client.on('end', function() 
	{  
		clientList.splice(clientList.indexOf(client), 1);
		broadcast(clientNick + ' has left the chat');
		console.log(clientNick + ' has left the chat');
	}) 
	
    client.on('error', function(e) 
	{  
		console.log(e);  
		broadcast(clientNick + ' has left the chat');
	});
});  

function broadcast(message) 
{  
	var cleanup = []
    for(var i = 0;i < clientList.length;i += 1) {      
		if(clientList[i].writable) 
		{
			clientList[i].write(message + '\n')
		} else 
		{  
			cleanup.push(clientList[i]) 
			clientList[i].destroy()
		}      
    }
  	for(i = 0;i < cleanup.length;i += 1) 
	{  
		clientList.splice(clientList.indexOf(cleanup[i]), 1)
	}
}  

chatServer.listen(5819, function() 
{ 
	console.log('Server is starting...');
});