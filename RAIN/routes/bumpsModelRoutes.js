var express = require('express');
var router = express.Router();
var bumpsModelController = require('../controllers/bumpsModelController.js');

/*
 * GET
 */
router.get('/', bumpsModelController.list);
router.get('/pregled_bumpov/list', bumpsModelController.showBumps)

/*
 * GET
 */
router.get('/:id', bumpsModelController.show);

/*
 * POST
 */
router.post('/', bumpsModelController.create);
//router.post('/bumps', bumpsModelController.create);

/*
 * PUT
 */
router.put('/:id', bumpsModelController.update);

/*
 * DELETE
 */
router.delete('/:id', bumpsModelController.remove);

module.exports = router;
