import os
import requests

# URL of the Swagger JSON
swagger_url = "http://localhost:8080/swagger/v1/swagger.json"

# Directory and file paths
infra_dir = "../infra"
output_file = os.path.join(infra_dir, "swagger.json")

# Fetch the Swagger JSON
try:
    response = requests.get(swagger_url)
    response.raise_for_status()  # Raise an error for bad responses

    # Write the content to the swagger.json file
    with open(output_file, "w") as f:
        f.write(response.text)

    print("swagger.json saved successfully in the infra directory!")

except requests.exceptions.RequestException as e:
    print(f"Error fetching Swagger JSON: {e}")
