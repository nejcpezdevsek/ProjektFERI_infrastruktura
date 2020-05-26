var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var trafficSituationSchema = new Schema({
	'cesta' : String,
	'lokacija' : String,
	'vzrok' : String
});

module.exports = mongoose.model('trafficSituation', trafficSituationSchema);
