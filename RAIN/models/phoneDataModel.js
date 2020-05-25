var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var phoneDataSchema = new Schema({
	'phone_name' : String,
	'gyroscope' : String,
	'accelerometer' : String,
	'image' : String,
	'latitude' : String,
	'longitude' : String
});

module.exports = mongoose.model('phoneData', phoneDataSchema);
