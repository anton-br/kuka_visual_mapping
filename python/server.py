from flask import Flask
from flask import request
import json

# ЗАПУСК СЕРВЕРА
# set FLASK_APP=server.py && flask run
app = Flask(__name__)

class Requester:
    def __init__(self):
        self.ready = 'None'
        self.image = 'None'
        self.pred_mask = 'None'
        self.odom = 'None'
req = Requester()

@app.route('/ready/', methods=['GET', 'POST'])
def ready():
    if request.method == 'POST':
        try:
            jsondata = request.get_json()
            data = json.loads(jsondata)
        except:
            jsondata = request.data
            data = json.loads(jsondata)

        req.ready = json.dumps(data)
        return json.dumps(data)

    else:
        is_ready = req.ready
        return is_ready

@app.route('/new_val/', methods=['GET', 'POST'])
def new_val():
    if request.method == 'POST':
        jsondata = request.get_json()
        data = json.loads(jsondata)
        req.pred_mask = json.dumps(data)
        return json.dumps(data)
    else:
        return req.pred_mask

@app.route('/new_image/', methods=['GET', 'POST'])
def new_img():
    if request.method == 'POST':
        jsondata = request.data
        data = json.loads(jsondata)
        req.image = json.dumps(data)
        return json.dumps(data)
    else:
        return req.image

@app.route('/odom/', methods=['GET', 'POST'])
def new_odom():
    if request.method == 'POST':
        jsondata = request.data
        data = json.loads(jsondata)
        req.odom = json.dumps(data)
        return json.dumps(data)
    else:
        return req.odom

if __name__ == '__main__':
    app.run(debug=True)
