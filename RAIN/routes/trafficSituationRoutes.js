var express = require('express');
var router = express.Router();
var trafficSituationController = require('../controllers/trafficSituationController.js');

/*
 * GET
 */
router.get('/', trafficSituationController.list);
router.get('/:id', trafficSituationController.show);
router.get('/pregled_podatkov/list', trafficSituationController.showPregledPodatkov)

/*
 * POST
 */
router.post('/', trafficSituationController.create);

/*
 * PUT
 */
router.put('/:id', trafficSituationController.update);

/*
 * DELETE
 */
router.delete('/:id', trafficSituationController.remove);

module.exports = router;
