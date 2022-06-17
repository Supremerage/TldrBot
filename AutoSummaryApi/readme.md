# Running the API
On startup the API will download the model and load it into memory. 
Afterwards the web server will be reachable at localhost:8000.

## Running locally
First install the requirements: `pip install -r requirements.txt`

Then the API can be started using `python -m uvicorn main:app --port 8000`

## Running in docker
The API can also be run using [docker](https://www.docker.com/):

`docker run -p 8000:8000 autosummary:latest`

# OpenAPI

SwaggerUI is available at `/docs`

OpenAPI json specification is available at `/openapi.json`