var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var gpsDataSchema = new Schema({
	'latitude' : Number,
	'longitude' : Number
});

module.exports = mongoose.model('gpsData', gpsDataSchema);
