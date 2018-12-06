//https://shrouded-springs-66341.herokuapp.com/
// test #2

var express = require('express');
var pg = require("pg");
var app = express();

const config = {
    user: 'postgres',
    database: 'postgres',
    password: '0000',
    port: 5432,
    max: 10,
    idleTimeoutMillis: 30000
};

const pool = new pg.Pool(config);


app.get('/pool', function (req, res, next) {
    pool.connect(function (err, client, done) {
        if (err) {
            console.log("not able to get connection " + err);
            res.status(400).send(err);
        }
        client.query('SELECT * from serveraccounts', function (err, result) {
            done(); 
            if (err) {
                console.log(err);
                res.status(400).send(err);
            }
            res.status(200).send(result.rows);
        });
    });
});

app.post('/post', function(req, res, next) {
 pool.connect(function (err, client, done){
    if (err) {
        console.log("not able to get connection " + err);
        res.status(400).send(err);
    }
    client.query(`INSERT INTO serveraccounts (serverid, servername) VALUES (${req.body.serverid}, ${req.body.ServerName})`, function (err, result) {
        done(); 
        if (err) {
            console.log(err);
            res.status(400).send(err);
        }
        res.status(200).send(result.rows);
    });
 });
});

app.listen(3000, function () {
    console.log('Server is running.. on Port 3000');
});
