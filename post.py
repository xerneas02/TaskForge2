import requests

# API endpoint URL
api_url = "http://localhost:5227/api/Pokemon"

# Pokemon template data to be posted
pokemon_data = {
    "Nom": "Salameche",
    "Type": 1
}

# Send a POST request to create a Pokemon template
response = requests.post(api_url, json=pokemon_data)

# Check the response status
if response.status_code == 201:
    print("Pokemon template created successfully!")
    print("New Pokemon template data:", response.json())
else:
    print("Failed to create Pokemon template.")
    print("Response status code:", response.status_code)
    print("Response content:", response.text)