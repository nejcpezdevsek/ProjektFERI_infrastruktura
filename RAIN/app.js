var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');

//povezava z bazo
var mongoose = require('mongoose');
//Set up default mongoose connection
var mongoDB = 'mongodb://127.0.0.1:27017/vaja3';
mongoose.connect(mongoDB);
// Get Mongoose to use the global promise library
mongoose.Promise = global.Promise;
//Get the default connection
var db = mongoose.connection;

var indexRouter = require('./routes/index');
var usersRouter = require('./routes/users');
var phoneDataRouter = require('./routes/phoneDataRoutes');
var trafficSituationRouter = require('./routes/trafficSituationRoutes');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'hbs');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use('/css', express.static(__dirname + '/node_modules/bootstrap/dist/css'));

app.use('/', indexRouter);
app.use('/users', usersRouter);
app.use('/phonedata', phoneDataRouter);
app.use('/trafficsituation', trafficSituationRouter);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

app.use(express.static(path.join(__dirname, 'public')));
// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
