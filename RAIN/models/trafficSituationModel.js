var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var trafficSituationSchema = new Schema({
	'date' : Date,
	'road' : String,
	'description' : String
});

module.exports = mongoose.model('trafficSituation', trafficSituationSchema);
