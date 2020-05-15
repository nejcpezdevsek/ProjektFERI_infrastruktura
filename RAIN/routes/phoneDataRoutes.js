var express = require('express');
var router = express.Router();
var phoneDataController = require('../controllers/phoneDataController.js');

/*
 * GET
 */
router.get('/', phoneDataController.list);

/*
 * GET
 */
router.get('/:id', phoneDataController.show);

/*
 * POST
 */
router.post('/', phoneDataController.create);

/*
 * PUT
 */
router.put('/:id', phoneDataController.update);

/*
 * DELETE
 */
router.delete('/:id', phoneDataController.remove);

module.exports = router;
