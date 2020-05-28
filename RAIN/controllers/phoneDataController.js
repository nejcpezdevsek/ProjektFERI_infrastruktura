var phoneDataModel = require('../models/phoneDataModel.js');
//var spawn = require('child_process').spawn
/**
 * phoneDataController.js
 *
 * @description :: Server-side logic for managing phoneDatas.
 */
module.exports = {

    /**
     * phoneDataController.list()
     */
    list: function (req, res) {
        // spawn new child process to call the python script
        // collect data from script
        const spawn = require('child_process').spawn
        const python = spawn('python', ['C:\\Users\\nejci\\OneDrive - Univerza v Mariboru\\FAKS\\PROJEKTI\\RAIN\\controllers\\hello.py']);
        python.stdout.on('data', function (data) {
            console.log('Pipe data from python script ...');
            console.log(data.toString())
           });

        phoneDataModel.find(function (err, phoneDatas) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting phoneData.',
                    error: err
                });
            }
            return res.render("zajete_slike/list", {phoneDatas:phoneDatas});
        });
    },

    /**
     * phoneDataController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        phoneDataModel.findOne({_id: id}, function (err, phoneData) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting phoneData.',
                    error: err
                });
            }
            if (!phoneData) {
                return res.status(404).json({
                    message: 'No such phoneData'
                });
            }
            return res.render("zajete_slike/show", phoneData);
        });
    },

    /**
     * phoneDataController.showZajeteSlike()
     */
    showZajeteSlike: function (req, res) {
        res.render('zajete_slike/list');
    }
    ,

    /**
     * phoneDataController.create()
     */
    create: function (req, res) {
        var phoneData = new phoneDataModel({
			phone_name : req.body.phone_name,
			gyroscope : req.body.gyroscope,
			accelerometer : req.body.accelerometer,
			image : req.body.image,
            latitude : req.body.latitude,
            longitude : req.body.longitude
        });

        phoneData.save(function (err, phoneData) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when creating phoneData',
                    error: err
                });
            }
            return res.status(201).json(phoneData);
        });
    },


    /**
     * phoneDataController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        phoneDataModel.findOne({_id: id}, function (err, phoneData) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting phoneData',
                    error: err
                });
            }
            if (!phoneData) {
                return res.status(404).json({
                    message: 'No such phoneData'
                });
            }

            phoneData.phone_name = req.body.phone_name ? req.body.phone_name : phoneData.phone_name;
			phoneData.gyroscope = req.body.gyroscope ? req.body.gyroscope : phoneData.gyroscope;
			phoneData.accelerometer = req.body.accelerometer ? req.body.accelerometer : phoneData.accelerometer;
			phoneData.image = req.body.image ? req.body.image : phoneData.image;
			
            phoneData.save(function (err, phoneData) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating phoneData.',
                        error: err
                    });
                }

                return res.json(phoneData);
            });
        });
    },

    /**
     * phoneDataController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        phoneDataModel.findByIdAndRemove(id, function (err, phoneData) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the phoneData.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
