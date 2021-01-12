var mongoose = require('mongoose');
var Schema   = mongoose.Schema;

var bumpsModelSchema = new Schema({
	'name' : String,
	'date' : Date,
	'bumps' : Number
});

module.exports = mongoose.model('bumpsModel', bumpsModelSchema);
