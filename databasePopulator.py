import http.client
import urllib.parse
import json
conn = http.client.HTTPConnection("localhost", 5152)
# Set the headers
headers = {
    'Content-type': 'application/json'
}
if False:
    # Creates new static
    endpoint = "/api/Static"
    params = "name=SussyStatic"
    url = f"{endpoint}?{params}"

    # Send the GET request
    conn.request("POST", url)
    conn.close()

if True:

    NAMEList = ["Leonhard Euler", "Apologizing Canadian", "Daibumu Kasai", "BigDog", "Sylvester Stalon", "Yna", "Harrow Levesque", "Sol"]
    RoleList = [1, 6, 9, 16, 18, 4, 14, 8]

    # Edit player
    for i in range(8):
        conn = http.client.HTTPConnection("localhost", 5152)
        params = json.dumps({
            "id": i+1,
            "useBis": True,
            "gearToChange": 1,
            "newGearId": 0,
            "newEtro": "string",
            "newName": NAMEList[i],
            "newJob": RoleList[i],
            "newLock": True
            })



        # Send the PUT request with the parameters
        conn.request("PUT", "/api/Player/NewName", body=params, headers=headers)
        response = conn.getresponse()

        # Print the response status code and content
        print(response.status)
        print(response.read().decode())
        conn.close()
        conn = http.client.HTTPConnection("localhost", 5152)
        conn.request("PUT", "/api/Player/NewJob", body=params, headers=headers)
        response = conn.getresponse()

        # Print the response status code and content
        print(response.status)
        print(response.read().decode())
        conn.close()

# To populate the Gear DAtabase use the function of the API. Make sure to add a 'No Equipment' gear that has id 1 before populating.


# Close the connection
conn.close()