var phoneDataModel = require('../models/phoneDataModel.js');

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

        const spawn = require('child_process').spawn
        const python = spawn('python', ['../ORV/DetekcijaZnakov/skripta/sign_recog.py',req.body.image]);

        python.stdout.on('data', function (data) {
            console.log('Pipe data from python script ...');
            console.log(data.toString())
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
