var express = require('express');
var router = express.Router();
var phoneDataController = require('../controllers/phoneDataController.js');

var multer = require('multer');
var upload = multer({ dest: 'public/images/' });

/*
 * GET
 */
router.get('/', phoneDataController.list);
router.get('/:id', phoneDataController.show);
//router.get('/zajete_slike/list', phoneDataController.showZajeteSlike);

/*
 * POST
 */
router.post('/', upload.single("image"), phoneDataController.create);

/*
 * PUT
 */
router.put('/:id', phoneDataController.update);

/*
 * DELETE
 */
router.delete('/:id', phoneDataController.remove);

module.exports = router;
