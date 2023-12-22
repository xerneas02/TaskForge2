from bs4 import BeautifulSoup
import requests

# API endpoint URL
api_url = "http://localhost:5227/api/Pokemon"

# Specify the encoding when reading the file
with open("Pokemon.html", "r", encoding="utf-8") as file:
    html_content = file.read()

# Create a BeautifulSoup object
soup = BeautifulSoup(html_content, 'html.parser')

# Find all table rows
rows = soup.find_all('tr')

pokemon_type_dict = {
    'Normal': 1,
    'Feu': 2,
    'Eau': 3,
    'Électrik': 4,
    'Plante': 5,
    'Glace': 6,
    'Combat': 7,
    'Poison': 8,
    'Sol': 9,
    'Vol': 10,
    'Psy': 11,
    'Insecte': 12,
    'Roche': 13,
    'Spectre': 14,
    'Dragon': 15,
    'Ténèbres': 16,
    'Acier': 17,
    'Fée': 18
}

# Iterate through each row
id = 1
for row in rows:
    try:
        # Extract the ID (assumed to be in the 3rd td)
        pokemon_name = row.find_all('td')[2].text.strip().split('\n')[0]

        # Extract the type (assumed to be in the 8th td, the 1st span, and the title of the first 'a')
        type_span = row.find_all('td')[7].find('span')
        type_title = type_span.a['title'][:-7] if type_span else "N/A"

        # Print the results (you can modify this part based on your requirement)
        print(f"ID: {id}, Name: {pokemon_name}, Type: {type_title}")
        pokemon_data = {
            "Id": id,
            "Nom": pokemon_name,
            "Type": pokemon_type_dict[type_title]
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
        
        id += 1
    except Exception as e:
        pass
