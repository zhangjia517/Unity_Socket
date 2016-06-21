var net = require('net');  
var chatServer = net.createServer(), clientList = [];  
      
chatServer.on('connection', function(client) {  
  client.name = client.remotePort;     
  clientList.push(client);   
  broadcast('welcome ' + client.name + '!', client);
  console.log('welcome ' + client.name + '!');
  
  client.on('data', function(data) {      
     broadcast(data, client);
  });  
  client.on('end', function() {  
    clientList.splice(clientList.indexOf(client), 1);
  }) 
  client.on('error', function(e) {  
  console.log(e);  
  });
});  

function broadcast(message, client) {  
	var cleanup = []
    for(var i=0;i<clientList.length;i+=1) {      
      if(clientList[i].writable) {
        clientList[i].write(client.name + " says " + message + '\n')  
      } else {  
        cleanup.push(clientList[i]) 
        clientList[i].destroy()  
      }      
    }    
  	for(i=0;i<cleanup.length;i+=1) 
	{  
		clientList.splice(clientList.indexOf(cleanup[i]), 1)
	}
}  

chatServer.listen(5819,'192.168.16.150', function() { 
  console.log('server 9000 on on on');
});