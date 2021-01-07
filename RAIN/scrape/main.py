import requests
from bs4 import BeautifulSoup
import json
import time
from mpi4py import MPI

def scrape():
    try:
        headers = {'User-Agent': 'whatever'}
        result = requests.get("https://www.rtvslo.si/stanje-na-cestah", headers=headers)
    except requests.ConnectionError as e:
        print(e)

    src = result.content
    soup = BeautifulSoup(src, "html.parser")
    found = soup.findAll('strong')
    cesta = []
    lok = []
    vzrok = []
    data = []

    comm = MPI.COMM_WORLD
    rank = comm.Get_rank()

    if rank == 0:
        for i in found:
            temp = i.text.split(',')
            if len(temp) == 2 and len(temp[1].split(':')) == 2:
                cesta.append(temp[0])
        comm.send(cesta, dest=3)
    elif rank == 1:
        for i in found:
            temp = i.text.split(',')
            if len(temp) == 2 and len(temp[1].split(':')) == 2:
                lok.append(temp[1].split(':')[0])
        comm.send(lok, dest=3)
    elif rank == 2:
        for i in found:
            temp = i.text.split(',')
            if len(temp) == 2 and len(temp[1].split(':')) == 2:
                vzrok.append(temp[1].split(':')[1])
        comm.send(vzrok, dest=3)
    elif rank == 3:
        data = []
        cesta = comm.recv(source=0)
        lok = comm.recv(source=1)
        vzrok = comm.recv(source=2)
        for i in range(0, len(cesta)):
            data.append({
                'cesta': cesta[i],
                'lokacija': lok[i],
                'vzrok': vzrok[i]
            })
    #with open('data.json', 'w') as outfile:
        #json.dump(data, outfile)

    #send a post request with the json file to the website
    try:
        response = requests.post('http://localhost:3000/trafficsituation', json=data)
        #print(response.status_code)
    except requests.exceptions.RequestException as e:
        print(e)
     #   raise SystemExit(e)
    

scrape()
