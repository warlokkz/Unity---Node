var io = require( 'socket.io' )( process.env.PORT || 3000 );
var shortId = require( 'shortId' );
console.log( 'server started' );

var players = [];

io.on( 'connection', function( socket ) {
  var thisPlayerId = shortId.generate();

  var player = {
    id: thisPlayerId,
    x: 0, 
    z: 0
  };

  players[thisPlayerId] = player;
  console.log( thisPlayerId + ' connected' );

  socket.emit( 'register', { id: thisPlayerId } );
  socket.broadcast.emit( 'spawn', { id: thisPlayerId } );
  socket.broadcast.emit( 'requestPosition' );

  for(var playerId in players) {
    if ( playerId === thisPlayerId ) continue;
    socket.emit( 'spawn', players[playerId] );
  }

  socket.on( 'move', function( data ) {
    data.id = thisPlayerId;

    player.x = data.x;
    player.z = data.z;

    socket.broadcast.emit( 'move', data );
  });

  socket.on( 'updatePosition', function( data ) {
    data.id = thisPlayerId;
    socket.broadcast.emit( 'updatePosition', data );
  });

  socket.on( 'follow', function( data ) {
    data.id = thisPlayerId;
    console.log("update position: ", data );
    socket.broadcast.emit( 'follow', data );
  });

  socket.on('disconnect', function ( data ) {
    delete players[thisPlayerId];
    socket.broadcast.emit( 'disconnected', { id: thisPlayerId } );
  });

});