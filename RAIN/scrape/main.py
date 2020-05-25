import requests
from bs4 import BeautifulSoup
import json


def scrape():
    try:
        result = requests.get("httaps://www.rtvslo.si/stanje-na-cestah")
    except requests.ConnectionError as e:
        print(e)

    src = result.content
    soup = BeautifulSoup(src, "html.parser")
    found = soup.findAll('strong')
    cesta = []
    lok = []
    vzrok = []
    for i in found:
        temp = i.text.split(',')
        if len(temp) == 2:
            cesta.append(temp[0])
            lok.append(temp[1])
            vzr = temp[1].split(':')
            vzrok.append(vzr[1])

    data = {}
    data['nesrece'] = []
    #print(len(cesta))
    for i in range(0, len(cesta)):
        data['nesrece'].append({
            'cesta': cesta[i],
            'lokacija': lok[i],
            'vzrok': vzrok[i]
        })
    #with open('data.json', 'w') as outfile: 
        #json.dump(data, outfile)


    #Send a post request with the json file to the website
    try:
        response = requests.post('localhost:3000/trafficsituation', json=data)
        #print(response.status_code)
    except requests.exceptions.RequestException as e:
        print(e)
        raise SystemExit(e)

scrape()
