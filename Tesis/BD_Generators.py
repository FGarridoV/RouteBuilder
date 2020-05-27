import pandas as pd
import geopandas as gpd
import datetime as dt
from shapely.geometry import Point, LineString
import math
from copy import deepcopy


class Tools:
    
    @staticmethod
    def str_time(strTime):
        strTime = strTime.strip()
        if strTime[-2:] == '60':
            strTime = strTime[0:-2] + '59'
            time = dt.datetime.strptime(strTime, '%H:%M:%S')
            time += dt.timedelta(minutes=1)
        else:
            time = dt.datetime.strptime(strTime, '%H:%M:%S')
        return time.time()
    
    @staticmethod
    def point_node(df, node):
        return df.loc[df['node'] == node, 'geometry'].iloc[0]
    
    @staticmethod
    def time_gap(t, gap = 15):
        times = []
        date = dt.date(1, 1, 1)
        current = dt.datetime.combine(date, dt.time(6, 0, 0))
        end = dt.datetime.combine(date, dt.time(7, 30, 0))

        while current <= end:
            times.append(current)
            current += dt.timedelta(minutes=15)

        for i, time in enumerate(times):
            if t < time:
                return times[i-1]
            
            
class Vehicle:
    
    max_routes = 0
    
    def __init__(self, license_plate):
        self.license_plate = license_plate
        self.first_view = None
        self.routes = []
        self.chosen_route = None
    
    def add_new_route(self, route):
        self.routes.append(route)
        
    def set_chosen_route(self, route):
        for i, r in enumerate(self.routes):
            if r.nodes == route:
                self.chosen_route = i
                break
                
    def update_max_routes(self):
        if len(self.routes) > Vehicle.max_routes:
            Vehicle.max_routes = len(self.routes)
                
    def set_first_view(self, t):
        self.first_view = t
    
    def set_chosen_route_non_vehicles(self, pos):
        self.chosen_route = pos
        
        
class Route:
    
    num = 0
    def __init__(self, nodes, prob):
        self.num = Route.num
        self.nodes = nodes
        self.tesis_prob = prob
        self.n_of_nodes = len(nodes)-2
        self.time = None
        self.distance = None
        self.angular_cost = None
        Route.num += 1
        
    def set_time(self, time):
        self.time = time
    
    def set_distance(self, distance):
        self.distance = distance
    
    def set_angular_cost(self, angular_cost):
        self.angular_cost = angular_cost
        

class Network:
    
    def __init__(self, path_base):
        self.nodes = self.set_nodes(path_base)
        self.real_links = self.set_real_links(path_base)
        self.links = pd.DataFrame(columns= ['link', 'o', 'd'])
        self.define_links_based_on_bt_nodes()
        self.add_geometry_links()
    
    def set_nodes(self, path_base):
        nodesFile = path_base.replace('*', 'nodes.ty')
        dfn = pd.read_csv(nodesFile, names = ['node', 'x', 'y', 'bt'], skiprows=1)
        dfn['geometry'] = dfn.apply(lambda row: Point(row['x'], row['y']), axis = 1)
        return gpd.GeoDataFrame(dfn)
    
    def set_real_links(self, path_base):
        linksFile = path_base.replace('*', 'links.ty')
        dfl = pd.read_csv(linksFile, names = ['link', 'o', 'd'], skiprows=1)
        dfl['geometry'] = dfl.apply(lambda row: LineString([Tools.point_node(self.nodes, row['o']), Tools.point_node(self.nodes, row['d'])]), axis = 1)
        return gpd.GeoDataFrame(dfl)
    
    def add_geometry_links(self):
        self.links['geometry'] = self.links.apply(lambda row: LineString([Tools.point_node(self.nodes, row['o']), Tools.point_node(self.nodes, row['d'])]), axis = 1)
        self.links = gpd.GeoDataFrame(self.links)
    
    def outer_links(self, node):
        outer_links = self.real_links.loc[self.real_links['o'] == node]
        return outer_links
        
    def show(self):
        nmap = self.nodes.loc[self.nodes['bt']==1]
        lmap = self.links
        ax = lmap.plot(figsize = (13,13), linewidth = 3, zorder = -1, color = '#000000' )
        nmap.plot(ax = ax, figsize = (13,13), markersize = 2000, color = '#FF8155', zorder = 1)
        for x, y, label in zip(nmap.geometry.x, nmap.geometry.y, nmap['node']):
            ax.annotate(label, xy=(x, y), xytext=(-9, -5), textcoords="offset points")
            
    def search_new_links(self, node, visited_nodes):
        head_nodes = []
        visited_nodes = visited_nodes.copy()

        for i, link in self.outer_links(node).iterrows():
            if link['d'] in visited_nodes:
                continue
                
            visited_nodes.append(link['d'])
            if self.nodes.loc[self.nodes['node'] == link['d']]['bt'].iloc[0] == 0:
                head_nodes += self.search_new_links(link['d'], visited_nodes)
            else:
                head_nodes.append(link['d'])
        return head_nodes
  
    def define_links_based_on_bt_nodes(self):
        nodes = self.nodes.copy()
        k = 0
        for i, node in nodes.loc[nodes['bt'] == 1].iterrows():
            visited_nodes = []
            aux_nodes = self.search_new_links(node['node'],visited_nodes)
            
            for num in aux_nodes:
                if node['node'] != num:
                    link = {'link': k, 'o': node['node'], 'd': num}
                    self.links = self.links.append(link, ignore_index=True)
                    k += 1
    
    def get_nodes_list(self):
        nodes = self.nodes.loc[self.nodes['bt']==1]
        nodes_list = list(nodes['node'])
        return nodes_list
    
    def get_links_list(self):
        links_list = []
        for i, link in self.links.iterrows():
            links_list.append((link['o'],link['d']))
        return links_list
    
    
class Escenario:
    
    def __init__(self, network_code, inter_time, apriori_prob):
        Vehicle.max_routes = 0
        self.network_code = network_code
        self.inter_time = inter_time
        self.apriori_prob = apriori_prob
        self.network = Network(self.get_path('*'))
        self.dtimes = None
        self.ttimes = None
        self.vehicles = []
        self.non_vehicles = []
        print('    Reading vehicles ...')
        self.read_vehicles()
        print('    Vehicles read')
        print('    Reading times ...')
        self.times = self.read_times()
        print('    Times read')
        self.set_first_view_to_vehicles()
        print('    Creating and setting times dicts ...')
        self.set_times_dict()
        print('    Times dicts created and set')
        print('    Calculating angular, distance and time costs ...')
        self.set_angular_costs_and_distance()
        print('    Angular, distance and time costs calculated')
        print('    Creating non infenreceble_vehicles ...')
        self.create_non_infenrenceble_veh()
        print('    non inferenceble vehicles determinated ...')
        self.max_routes = Vehicle.max_routes
        
    def set_first_view_to_vehicles(self):
        for v in self.vehicles:
            lp = v.license_plate
            fv = self.times.loc[self.times['vehicle']==lp]['time'].min()
            date = dt.date(1, 1, 1)
            v.set_first_view(dt.datetime.combine(date, fv))

    def read_times(self):
        detBtFile = self.get_path('DetBT.ty')
        df = pd.read_csv(detBtFile, names = ['node', 'vehicle', 'time'])
        df['time'] = df.apply(lambda row: Tools.str_time(row['time']), axis = 1)
        bt_nodes = self.network.get_nodes_list()
        df = df.loc[df['node'].isin(bt_nodes)]
        return df
    
    def create_times_dict(self, gap = 15):
        nodes = self.network.get_nodes_list()
        links = self.network.get_links_list()

        self.dtimes = {}
        self.ttimes = {}

        date = dt.date(1, 1, 1)
        current = dt.datetime.combine(date, dt.time(6, 0, 0))
        end = dt.datetime.combine(date, dt.time(7, 30, 0))

        while current < end:

            for n in nodes:
                self.dtimes.update({(n, current): []})

            for l in links:
                self.ttimes.update({(l[0], l[1], current): []})

            current += dt.timedelta(minutes=gap)
            
    def avg_times(self):
        
        for t in self.dtimes:
            N = len(self.dtimes[t])
            if N > 0:
                self.dtimes[t] = sum(self.dtimes[t])/N
            else:
                self.dtimes[t] = 30
        
        for t in self.ttimes:
            N = len(self.ttimes[t])
            if N > 0:
                self.ttimes[t] = sum(self.ttimes[t])/N
            else:
                distance = self.network.links.loc[(self.network.links['o']== t[0]) & (self.network.links['d']== t[1])].iloc[0]['geometry'].length
                self.ttimes[t] = (distance/40)*3.6
                
    def set_times_dict(self):
        self.create_times_dict()
        vehicles = self.vehicles
        times = self.times
        links = self.network.links

        for v in vehicles:
            lp = v.license_plate
            detections = times.loc[times['vehicle']== lp]
            date = dt.date(1, 1, 1)
            previous_node = 0
            start_time = None
            end_time = None
            for i, det in detections.iterrows():   
                #INICIALIZACION
                if previous_node == 0:
                    previous_node = det['node']
                    start_time = dt.datetime.combine(date, det['time'])
                    end_time = dt.datetime.combine(date, det['time'])

                #SI EL NODO ACTUAL EL MISMO QUE EL ANTERIOR
                elif det['node'] == previous_node:
                    previous_node = det['node']
                    end_time = dt.datetime.combine(date, det['time'])

                #SI El NODO ACTUAL ES DISTINTO QUE EL ANTERIOR
                elif det['node'] != previous_node:
                    dwell_time = (end_time - start_time).total_seconds()
                    travel_time = (dt.datetime.combine(date, det['time'])-end_time).total_seconds()

                    if dwell_time!=0:
                        self.dtimes[(previous_node, Tools.time_gap(start_time))].append(dwell_time)

                    if len(links.loc[(links['o']== previous_node) & (links['d']== det["node"])]) == 1:
                        self.ttimes[(previous_node, det["node"], Tools.time_gap(end_time))].append(travel_time)

                    previous_node = det['node']
                    start_time = dt.datetime.combine(date, det['time'])
                    end_time = dt.datetime.combine(date, det['time'])
        self.avg_times()

    def read_vehicles(self):
        detailsFile = self.get_path('Details.txt')
        with open(detailsFile, 'r') as file:
            for i, line in enumerate(file):
        
                # New vehicle
                if line[0:11] == '---------- ':
                    lp = int(line.split(' ')[1])
                    v = Vehicle(lp)
                    self.vehicles.append(v)
        
                # New route in current vehicle
                elif line[0:6] == 'Route:':
                    route_prob = line[6:].split('//')
                    route = [int(x.strip()) for x in route_prob[0].split('>')]
                    prob = float(route_prob[1].split(':')[1].strip().replace(',','.'))
                    r = Route(route, prob)
                    self.vehicles[-1].add_new_route(r)
            
                # Route chosen by current vehicle
                elif line[0].isdigit():
                    chosen = [int(x.strip()) for x in line.split('//')[0].split('>')]
                    self.vehicles[-1].set_chosen_route(chosen)
                    self.vehicles[-1].update_max_routes()
            
                else:
                    continue
                    
    def get_license_plates_non_inferenceble(self):
        lps_inf = []
        for v in self.vehicles:
            lps_inf.append(v.license_plate)
            
        lps_non_inf = self.times.loc[~self.times['vehicle'].isin(lps_inf)]['vehicle']
        lps_non_inf = list(set(lps_non_inf.to_list()))
        return lps_non_inf
    
    def find_routes_in_vehicles(self, o, d):
        routes = []
        for v in self.vehicles:
            if v.routes[0].nodes[0] == o and v.routes[0].nodes[-1] == d:
                for r in v.routes:
                    if r.nodes == v.routes[0].nodes:
                        continue
                    
                    if not r.nodes in [x.nodes for x in routes]:
                        r_copy = deepcopy(r)
                        routes.append(r_copy)
                    else:
                        break
        return routes
                
    def create_non_infenrenceble_veh(self):
        lps_non_inf = self.get_license_plates_non_inferenceble()
        for lp in lps_non_inf:

            v = Vehicle(lp)
            self.non_vehicles.append(v)
            
            non_times = self.times.loc[self.times['vehicle'].isin(lps_non_inf)]
            detections = non_times.loc[non_times['vehicle'] == lp]
            
            chosen_route = []
            for i, d in detections.iterrows():
        
                if len(chosen_route) == 0:
                    chosen_route.append(d['node'])
                    date = dt.date(1, 1, 1)
                    v.set_first_view(dt.datetime.combine(date, d['time']))
        
                elif d['node'] != chosen_route[-1]:
                    chosen_route.append(d['node'])
    
            if len(chosen_route) > 1:
                r = Route(chosen_route, 1)
                self.non_vehicles[-1].add_new_route(r)
                self.non_vehicles[-1].set_chosen_route_non_vehicles(0)
            
                origin = self.non_vehicles[-1].routes[0].nodes[0]
                destination = self.non_vehicles[-1].routes[0].nodes[-1]
                other_routes = self.find_routes_in_vehicles(origin, destination)
            
                v.routes += other_routes
                self.non_vehicles[-1].update_max_routes()
            
            else:
                self.non_vehicles.pop()
                
            
        self.set_angular_costs_and_distance_chosen_non_vehicles()

        
    def set_angular_costs_and_distance(self):
        for v in self.vehicles:
            for r in v.routes:
                angular_cost_aux = 0
                distance_aux = 0
                time_aux = 0
                sink_node = r.nodes[-1]
                for i, n in enumerate(r.nodes[:-1]):
                    origin_node = r.nodes[i]
                    destination_node = r.nodes[i+1]
                    values = self.angular_cost_and_distance(origin_node, destination_node, sink_node)
                    angular_cost_aux += values[0]
                    distance_aux += values[1]
                    time_aux += self.dtimes[(r.nodes[i], Tools.time_gap(v.first_view))]
                    time_aux += self.ttimes[(r.nodes[i], r.nodes[i+1], Tools.time_gap(v.first_view))]
                time_aux += self.dtimes[(r.nodes[-1], Tools.time_gap(v.first_view))]
                r.set_angular_cost(angular_cost_aux)
                r.set_distance(distance_aux)
                r.set_time(time_aux)
    
    def set_angular_costs_and_distance_chosen_non_vehicles(self):
        for v in self.non_vehicles:
            for r  in v.routes:
                angular_cost_aux = 0
                distance_aux = 0
                time_aux = 0
                sink_node = r.nodes[-1]
                for i, n in enumerate(r.nodes[:-1]):
                    origin_node = r.nodes[i]
                    destination_node = r.nodes[i+1]
                    values = self.angular_cost_and_distance(origin_node, destination_node, sink_node)
                    angular_cost_aux += values[0]
                    distance_aux += values[1]
                    time_aux += self.dtimes[(r.nodes[i], Tools.time_gap(v.first_view))]
                    time_aux += self.ttimes[(r.nodes[i], r.nodes[i+1], Tools.time_gap(v.first_view))]
                time_aux += self.dtimes[(r.nodes[-1], Tools.time_gap(v.first_view))]
                r.set_angular_cost(angular_cost_aux)
                r.set_distance(distance_aux)
                r.set_time(time_aux)
                
    def angular_cost_and_distance(self, o_node, d_node, s_node):
        tail_node = self.network.nodes.loc[self.network.nodes['node'] == o_node]
        head_node = self.network.nodes.loc[self.network.nodes['node'] == d_node]
        dest_node = self.network.nodes.loc[self.network.nodes['node'] == s_node]
        
        p1 = (tail_node['geometry'].iloc[0].x, tail_node['geometry'].iloc[0].y)
        p2 = (head_node['geometry'].iloc[0].x, head_node['geometry'].iloc[0].y)
        p3 = (dest_node['geometry'].iloc[0].x, dest_node['geometry'].iloc[0].y)

        a = math.sqrt((p2[1] - p1[1])**2 + (p2[0] - p1[0])**2)
        b = math.sqrt((p3[1] - p1[1])**2 + (p3[0] - p1[0])**2)
        c = math.sqrt((p2[1] - p3[1])**2 + (p2[0] - p3[0])**2)
        
        if p2 != p3 and p1 != p3:
            cosAngle = (a**2 + b**2 - c**2) / (2 * a * b)
            alfa = math.acos(cosAngle)
            ang_cost = a * math.sin(alfa / 2)
        else:
            ang_cost = 0
        distance = a
        
        return (ang_cost, distance)
        
    def get_time(self, vehicle, origin, destination):
        df_veh = self.times.loc[self.times['vehicle'] == vehicle]
        origin_time = df_veh.loc[df_veh['node']== origin].min()
        destination_time = df_veh.loc[df_veh['node']== destination].max()
        date = dt.date(1, 1, 1)
        origin_time = dt.datetime.combine(date, origin_time)
        destination_time = dt.datetime.combine(date, destination_time)
        time = destination_time - origin_time
        return time.total_seconds()
    
    def get_path(self, data):
        return 'scenarios/{}/{} seg/{}/{}'.format(self.network_code, self.inter_time, self.apriori_prob, data)
               
    def show_network(self):
        self.network.show()
        
networks = ['18h', '18u', '27', '36']
inter_times = [1, 5, 8]
apriori_probs = [2]

scenarios = {}
for n in networks:
    for t in inter_times:
        for p in apriori_probs:
            print('Escenario {}:{}:{}'.format(n,t,p))
            esc = Escenario(n,t,p)
            scenarios.update({'{}:{}:{}'.format(n,t,p) : esc})

            
for e in scenarios:
    sc = scenarios[e]
    opc = sc.vehicles[0].max_routes
    base_columns = ['tesis_prob_{}', 'n_nodes_{}', 'time_{}', 'distance_{}', 'angular_cost_{}']
    cols = []
    for i in range(opc):
        for c in base_columns:
            cols.append(c.format(i))
        
    df_inf = pd.DataFrame(columns = cols)
    df_ninf = pd.DataFrame(columns = cols)
    
    for v in sc.vehicles:
        row = {}
        chosen = v.chosen_route
        if chosen == None:
            continue
        
        #Guardamos la chosen
        r = v.routes[chosen]
        row['tesis_prob_0'] = r.tesis_prob
        row['n_nodes_0'] = r.n_of_nodes
        row['time_0'] = r.time
        row['distance_0'] = r.distance 
        row['angular_cost_0'] = r.angular_cost
        
        #Guardamos el resto
        k = 1
        for i, r in enumerate(v.routes):
            if i != chosen:
                row['tesis_prob_{}'.format(k)] = r.tesis_prob
                row['n_nodes_{}'.format(k)] = r.n_of_nodes
                row['time_{}'.format(k)] = r.time
                row['distance_{}'.format(k)] = r.distance 
                row['angular_cost_{}'.format(k)] = r.angular_cost
                k+=1
        df_inf = df_inf.append(row, ignore_index=True)

    for v in sc.non_vehicles:
        row = {}
        chosen = v.chosen_route
        if chosen == None:
            continue
        
        #Guardamos la chosen
        r = v.routes[chosen]
        row['tesis_prob_0'] = r.tesis_prob
        row['n_nodes_0'] = r.n_of_nodes
        row['time_0'] = r.time
        row['distance_0'] = r.distance 
        row['angular_cost_0'] = r.angular_cost
        
        #Guardamos el resto
        k = 1
        for i, r in enumerate(v.routes):
            if i != chosen:
                row['tesis_prob_{}'.format(k)] = r.tesis_prob
                row['n_nodes_{}'.format(k)] = r.n_of_nodes
                row['time_{}'.format(k)] = r.time
                row['distance_{}'.format(k)] = r.distance 
                row['angular_cost_{}'.format(k)] = r.angular_cost
                k+=1
        df_ninf = df_ninf.append(row, ignore_index=True)
        
    df_inf.to_csv('infs/inf_{}.csv'.format(e),index= False)
    df_ninf.to_csv('no_infs/ninf_{}.csv'.format(e),index= False)