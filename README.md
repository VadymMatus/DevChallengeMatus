# How to run dummy project template

1. Clone repository: \
    `git clone https://github.com/SimCorp/DevChallenge`
2. Go to directory *DevChallenge/SC.DevChallenge.Api*
3. Publish release version of API: \
    `dotnet publish -c Release`
4. Build Docker image: \
    `docker build -t sc-dev-challenge .`
5. Run Docker image: \
    `docker run -it --rm -p 5000:80 sc-dev-challenge`
6. Check your API by calling GET method:
    - Browser: <http://localhost:5000/api/prices/average>
    - curl: `curl -X GET "http://localhost:5000/api/prices/average" -H  "accept: text/plain"`
