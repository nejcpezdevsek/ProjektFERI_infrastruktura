var express = require('express');
var router = express.Router();
var trafficSituationController = require('../controllers/trafficSituationController.js');

/*
 * GET
 */
router.get('/', trafficSituationController.list);

/*
 * GET
 */
router.get('/:id', trafficSituationController.show);

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
