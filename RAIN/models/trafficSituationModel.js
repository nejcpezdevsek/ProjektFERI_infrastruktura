var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var trafficSituationSchema = new Schema({
	'ime' : String,
	'priimek' : String,
	'starost' : String
});

module.exports = mongoose.model('trafficSituation', trafficSituationSchema);
