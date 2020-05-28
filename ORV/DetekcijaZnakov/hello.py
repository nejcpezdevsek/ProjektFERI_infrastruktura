import sys
import base64

imgdata = base64.b64decode(sys.argv[1])
filename = '../ORV/DetekcijaZnakov/slika.jpg'  # I assume you have a way of picking unique filenames
with open(filename, 'wb') as f:
    f.write(imgdata)