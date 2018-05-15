from flask import Flask
from flask import request
import json
from svd_python import Requester

# ЗАПУСК СЕРВЕРА
# set FLASK_APP=server.py && flask run
app = Flask(__name__)
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
        mask = req.pred_mask
        return mask

@app.route('/new_image/', methods=['GET', 'POST'])
def new_img():
    if request.method == 'POST':
        jsondata = request.data
        data = json.loads(jsondata)
        req.image = json.dumps(data)
        return json.dumps(data)
    else:
        image = req.image
        return image

if __name__ == '__main__':
    app.run(debug=True)
