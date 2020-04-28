from AAPI import *
from PyANGKernel import *
import sys
from random import *
import math

#Variables
nVehBT = 0
vehsID_IN = []
nNodes = 0
nodesID = []
nLinks = 0
linksID = []
radio = 25
ratio = 10
linksIDAssoNodesID = []
nodesCoord = []
AllBT = open("/Users/Francisco/Desktop/AllBT"+ str(ratio) +".ty","w")
DetBT = open("/Users/Francisco/Desktop/DetBT"+ str(ratio) +".ty","w")

def AAPILoad():
	return 0

def AAPIInit():
	start_number_nodes_And_links()
	complete_list_assosiation()
	obtain_nodes_coords()
	write_nodes()
	AKIPrintString("File nodes.ty was created in Desktop")
	write_links()
	AKIPrintString("File links.ty was created in Desktop")
	AKIPrintString('Starting simulation...')
	return 0

def AAPIManage(time, timeSta, timTrans, SimStep):
	if float(time).is_integer():
		hour = int(math.floor(timeSta/3600))
		minute = int(math.floor((timeSta/3600-hour)*60))
		second = int(timeSta - hour*3600 - minute*60)
		if len(str(hour))==1:
			hour = "0" + str(hour)
		if len(str(minute))==1:
			minute = "0" + str(minute)
		if len(str(second))==1:
			second = "0" + str(second)
		timeStamp = str(hour) + ":" + str(minute) + ":" + str(second)

		for i in range(0,nNodes):
			atractives = atractivesID(nodesID[i])
			potentials = potentialsID(nodesID[i],atractives)
			detecteds = detectedsID(potentials)

			#All BT vehicles
			for j in range(0,len(potentials)):
				AllBT.write(str(nodesID[i]) + ", " + str(potentials[j]) + ", " + timeStamp)
				AllBT.write("\n")
			#Detected BT vehicles
			if time%ratio==0:
				for j in range(0,len(detecteds)):
					DetBT.write(str(nodesID[i]) + ", " + str(detecteds[j]) + ", " + timeStamp)
					DetBT.write("\n")
	return 0

def AAPIPostManage(time, timeSta, timTrans, SimStep):

	return 0

def AAPIFinish():
	AllBT.close()
	AKIPrintString("File AllBT.ty was created in Desktop")
	DetBT.close()
	AKIPrintString("File DetBT.ty was created in Desktop")
	AKIPrintString("Simulation Ended")
	AKIPrintString("------------------------------------------")
	AKIPrintString("File Gen RouteBuilder is a registered trademark of TyggerSoftware Inc. 2017, a non-profit organization. - All rights reserved")
	return 0

def AAPIUnLoad():
    return 0

def AAPIEnterVehicle(idveh,idsection):
	global nVehBT, vehsID_IN
	IDin = int(str(idveh))
	AKIVehSetAsTracked(IDin)
	carType = AKIVehTrackedGetInf(IDin).type
	if carType==1:
		nVehBT+=1
		vehsID_IN.append(IDin)

def AAPIExitVehicle(idveh,idsection):
	global nVehBT, vehsID_IN
	IDout = int(str(idveh))
	AKIVehSetAsTracked(IDout)
	carType = AKIVehTrackedGetInf(IDout).type
	if carType==1:
		nVehBT-=1
		vehsID_IN.remove(IDout)


#Mis metodos

def BTveh_from_section_id(idsection):
	global vehsID_IN
	ids = int(str(idsection))
	lista = []
	nb = AKIVehStateGetNbVehiclesSection(ids,True)
	for i in range(0,nb):
		infVeh = AKIVehStateGetVehicleInfSection(ids,i)
		valID = int(str(infVeh.idVeh))
		lista.append(valID)
	listaBT = []
	for i in range(0,len(lista)):
		if lista[i] in vehsID_IN:
			listaBT.append(lista[i])
	return listaBT

def BTveh_from_junction_id(idjunction):
	global vehsID_IN
	idj = int(str(idjunction))
	lista = []
	nb = AKIVehStateGetNbVehiclesJunction(idj)
	for i in range(0,nb):
		infVeh = AKIVehStateGetVehicleInfJunction(idj,i)
		valID = int(str(infVeh.idVeh))
		lista.append(valID)
	listaBT = []
	for i in range(0,len(lista)):
		if lista[i] in vehsID_IN:
			listaBT.append(lista[i])
	return listaBT

def complete_list_assosiation():
	global nodesID, linksIDAssoNodesID
	for i in range(0,len(nodesID)):
		listAux = []
		turns = AKIInfNetGetNbTurnsInNode(nodesID[i])
		for j in range(0,turns):
			info = AKIInfNetGetTurnInfo(nodesID[i],j)
			if not info.originSectionId in listAux:
				listAux.append(info.originSectionId)
			if not info.destinationSectionId in listAux:
				listAux.append(info.destinationSectionId)
		linksIDAssoNodesID.append(listAux)

def atractivesID(idjunction):
	global nodesID, linksID, linksIDAssoNodesID
	lista = BTveh_from_junction_id(idjunction)
	pos = nodesID.index(idjunction)
	for i in range(0,len(linksIDAssoNodesID[pos])):
		lista.extend(BTveh_from_section_id(linksIDAssoNodesID[pos][i]))
	return lista

def potentialsID(idjunction, atractives):
	global radio, nodesID, nodesCoord
	lista = []
	pos = nodesID.index(idjunction)
	a = nodesCoord[pos][0]
	b = nodesCoord[pos][1]
	for i in range(0,len(atractives)):
		x=AKIVehTrackedGetInf(atractives[i]).xCurrentPos
		y=AKIVehTrackedGetInf(atractives[i]).yCurrentPos
		if (x-a)**2 + (y-b)**2 <= radio**2:
			lista.append(atractives[i])
	return lista

def detectedsID(potentials):
        lista = []
        for i in range(0,len(potentials)):
            speed = AKIVehTrackedGetInf(potentials[i]).CurrentSpeed
            if is_detected(speed):
                lista.append(potentials[i])
        return lista

def obtain_nodes_coords():
	global nodesID, nodesCoord
	model = GKSystem.getSystem().getActiveModel()

	for i in range(0,len(nodesID)):
		nodeAux = model.getCatalog().find(nodesID[i])
		position = nodeAux.getPosition()
		pos = [float(position.x),float(position.y)]
		nodesCoord.append(pos)

def start_number_nodes_And_links():
	global nNodes, nLinks, nodesID, linksID, linksIDAssoNodesID
	nNodes = AKIInfNetNbJunctions()
	nLinks = AKIInfNetNbSectionsANG()
	for i in range(0,nNodes):
		print ""
		nodesID.append(AKIInfNetGetJunctionId(i))
	for i in range(0,nLinks):
		print ""
		linksID.append(AKIInfNetGetSectionANGId(i))

def p_from_v(speed):
    if speed>=0 and speed<=80:
        return -0.01*speed+0.8
    else:
        return 0

def is_detected(speed):
    p = p_from_v(speed)
    u = AKIGetRandomNumber()
    if u<=p:
        return True
    else:
        return False

def links_corrector():
	global nLinks, linksID, linksIDAssoNodesID
	aux = []
	for i in range(0,len(linksIDAssoNodesID)):
		aux.extend(linksIDAssoNodesID[i])
	for i in range(0,nLinks):
		aux.remove(linksID[i])
	return aux

def links_associator(linksR):
	linksVector = []
	model = GKSystem.getSystem().getActiveModel()
	for i in range(0,len(linksR)):
		linkAux = model.getCatalog().find(linksR[i])
		nodOrigin = int(linkAux.getOrigin().getId())
		nodDestination = int(linkAux.getDestination().getId())
		vector = [nodOrigin,nodDestination]
		linksVector.append(vector)
	return linksVector

def write_nodes():
	global nNodes, nodesID
	nodestxt = open("/Users/Francisco/Desktop/nodes"+ str(ratio) +".ty","w")
	nodestxt.write(str(nNodes))
	nodestxt.write("\n")
	for i in range(0,nNodes):
		nodestxt.write(str(nodesID[i])+", "+str(nodesCoord[i][0])+", "+str(nodesCoord[i][1])+", "+ "0")
		nodestxt.write("\n")
	nodestxt.close()

def write_links():
	linksR = links_corrector()
	vectors = links_associator(linksR)
	linkstxt = open("/Users/Francisco/Desktop/links"+ str(ratio) +".ty","w")
	linkstxt.write(str(len(linksR)))
	linkstxt.write("\n")
	for i in range(0,len(linksR)):
		linkstxt.write(str(linksR[i])+", "+str(vectors[i][0])+", "+str(vectors[i][1]))
		linkstxt.write("\n")
	linkstxt.close()
