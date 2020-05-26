var trafficSituationModel = require('../models/trafficSituationModel.js');

/**
 * trafficSituationController.js
 *
 * @description :: Server-side logic for managing trafficSituations.
 */
module.exports = {

    /**
     * trafficSituationController.list()
     */
    list: function (req, res) {
        trafficSituationModel.find(function (err, trafficSituations) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting trafficSituation.',
                    error: err
                });
            }
            return res.render("pregled_podatkov/list", trafficSituations);
        });
    },

    /**
     * trafficSituationController.show()
     */
    show: function (req, res) {
        var id = req.params.id;
        trafficSituationModel.findOne({_id: id}, function (err, trafficSituation) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting trafficSituation.',
                    error: err
                });
            }
            if (!trafficSituation) {
                return res.status(404).json({
                    message: 'No such trafficSituation'
                });
            }
            return res.json(trafficSituation);
        });
    },

    /**
     * trafficSituationController.showPregledPodatkov()
     */
    showPregledPodatkov: function (req, res) {
        res.render('pregled_podatkov/list');
    }
    ,

    /**
     * phoneDataController.showZajeteSlike()
     */
    showZajeteSlike: function (req, res) {
        res.render('zajete_slike/list');
   }
   ,

    /**
     * trafficSituationController.create()
     */
    create: function (req, res) {
        var trafficSituation;
        //console.log(req.body[0].cesta)
        req.body.forEach(element => {trafficSituation = new trafficSituationModel({
            cesta : element.cesta,
            lokacija : element.lokacija,
            vzrok : element.vzrok
        });

        trafficSituation.save(function (err, trafficSituation) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when creating trafficSituation',
                    error: err
                });
            }
        });});
        return res.status(201).json(trafficSituation);
    },

    /**
     * trafficSituationController.update()
     */
    update: function (req, res) {
        var id = req.params.id;
        trafficSituationModel.findOne({_id: id}, function (err, trafficSituation) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when getting trafficSituation',
                    error: err
                });
            }
            if (!trafficSituation) {
                return res.status(404).json({
                    message: 'No such trafficSituation'
                });
            }

            trafficSituation.date = req.body.date ? req.body.date : trafficSituation.date;
			trafficSituation.road = req.body.road ? req.body.road : trafficSituation.road;
			trafficSituation.description = req.body.description ? req.body.description : trafficSituation.description;
			
            trafficSituation.save(function (err, trafficSituation) {
                if (err) {
                    return res.status(500).json({
                        message: 'Error when updating trafficSituation.',
                        error: err
                    });
                }

                return res.json(trafficSituation);
            });
        });
    },

    /**
     * trafficSituationController.remove()
     */
    remove: function (req, res) {
        var id = req.params.id;
        trafficSituationModel.findByIdAndRemove(id, function (err, trafficSituation) {
            if (err) {
                return res.status(500).json({
                    message: 'Error when deleting the trafficSituation.',
                    error: err
                });
            }
            return res.status(204).json();
        });
    }
};
