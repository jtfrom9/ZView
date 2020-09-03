import firebase_admin
from firebase_admin import credentials

cred = credentials.Certificate("depth-mesh-firebase-adminsdk-38v0i-344ee0c985.json")
firebase_admin.initialize_app(cred, {
    'databaseURL': 'https://depth-mesh.firebaseio.com'
})

from firebase_admin import db
db.reference('db').delete()

