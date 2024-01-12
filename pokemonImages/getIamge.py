import os
import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin

# L'URL de la page
url = "https://pokemondb.net/pokedex/shiny"

# Faire la requête HTTP
response = requests.get(url)

# Vérifier si la requête a réussi (statut 200)
if response.status_code == 200:
    # Analyser le contenu HTML de la page
    soup = BeautifulSoup(response.text, 'html.parser')
    # Créer des dossiers pour stocker les images régulières et shiny
    if not os.path.exists("regular"):
        os.makedirs("regular")
    if not os.path.exists("shiny"):
        os.makedirs("shiny")

    # Sélectionner toutes les balises span avec la classe spécifiée
    spans = soup.find_all('span', class_='infocard-lg-img')
    # Parcourir chaque span
    for i, span in enumerate(spans):
        # Sélectionner le sprite régulier (première image)
        regular_img = span.find('img')
        regular_img_url = urljoin(url, regular_img['src'])
        
        # Télécharger le sprite régulier
        regular_img_data = requests.get(regular_img_url).content

        # Enregistrer le sprite régulier avec le nom spécifié dans le dossier regular
        filename = span.find_next('span', class_='infocard-lg-data text-muted').small.text
        regular_filepath = f"regular/{filename}.png"
        if not os.path.exists(regular_filepath):
            with open(regular_filepath, 'wb') as f:
                f.write(regular_img_data)

        # Sélectionner le sprite shiny (deuxième image)
        shiny_img = span.find_all('img')[1]
        shiny_img_url = urljoin(url, shiny_img['src'])

        # Télécharger le sprite shiny
        shiny_img_data = requests.get(shiny_img_url).content

        # Enregistrer le sprite shiny avec le nom spécifié dans le dossier shiny
        shiny_filepath = f"shiny/{filename}.png"
        if not os.path.exists(shiny_filepath):
            with open(shiny_filepath, 'wb') as f:
                f.write(shiny_img_data)

    print("Toutes les images ont été téléchargées avec succès.")
else:
    print(f"La requête a échoué avec le statut {response.status_code}.")
