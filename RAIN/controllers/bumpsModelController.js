var bumpsModelModel = require('../models/bumpsModelModel.js');

/**
 * bumpsModelController.js
 *
 * @description :: Server-side logic for managing bumpsModels.
 */
module.exports = {

    /**
     * bumpsModelController.list()
     */
    list: function (req, res) {
        bumpsModelModel.find(function (err, bumpsModels) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting bumpsModel.',
                    error: err
                });
            }
            return res.render("pregled_bumpov/list", bumpsModels);
        });
    },

    showBumps: function (req, res) {
        res.render('pregled_bumpov/list');
    }
    ,

    /**
     * bumpsModelController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        bumpsModelModel.findOne({_id: id}, function (err, bumpsModel) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting bumpsModel.',
                    error: err
                });
            }
            if (!bumpsModel) {
                return res.status(404).json({
                    message: 'No such bumpsModel'
                });
            }
            return res.json(bumpsModel);
        });
    },

    /**
     * bumpsModelController.create()
     */
    create: function (req, res) {
        var date = new Date()
        date.toLocaleDateString("sl-SL")
        var bumpsModel = new bumpsModelModel({
            name : req.body.name,
            date : date,			
            bumps : req.body.bumps
        });

        bumpsModel.save(function (err, bumpsModel) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when creating bumpsModel',
                    error: err
                });
            }
            return res.status(201).json(bumpsModel);
        });
    },

    /**
     * bumpsModelController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        bumpsModelModel.findOne({_id: id}, function (err, bumpsModel) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting bumpsModel',
                    error: err
                });
            }
            if (!bumpsModel) {
                return res.status(404).json({
                    message: 'No such bumpsModel'
                });
            }

            bumpsModel.name = req.body.name ? req.body.name : bumpsModel.name;
			bumpsModel.date = req.body.date ? req.body.date : bumpsModel.date;
			bumpsModel.bumps = req.body.bumps ? req.body.bumps : bumpsModel.bumps;
			
            bumpsModel.save(function (err, bumpsModel) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating bumpsModel.',
                        error: err
                    });
                }

                return res.json(bumpsModel);
            });
        });
    },

    /**
     * bumpsModelController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        bumpsModelModel.findByIdAndRemove(id, function (err, bumpsModel) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the bumpsModel.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
