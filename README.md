# Biking
Réalisé par Thomas Billequin

## Biking qu'est ce que c'est ?
  Biking est un projet scolaire ayant pour objectif la création de serveurs, de clients et l'utilisation d'API externes. Ainsi celui-ci s'articule autour de l'utilisation des données de JCDecaux. L'objectif est de créer une application qui permet de trouver le trajet entre deux adresses en déterminant si un vélo JCDecaux est nécessaire pour le trajet. Celle-ci renvoie ensuite les instructions pour le trajet optimal au client.

## Lancer le projet
### Lancer ActiveMQ
* Installer ActiveMQ
* Mettre ActiveMQ dans le PATH
* Ouvrir un terminal
* Lancer ActiveMQ avec la commande *activemq start*
### Lancer le serveur Soap
* Accéder au dossier *BikingServerSOAP\Biking\bin\Debug*
* Lancer le fichier *Biking.exe*

### Lancer le serveur Proxy Cache
* Accéder au dossier *BikingServerSOAP\JCDecauxCache\bin\Debug*
* Lancer le fichier *JCDecauxCache.exe*

### Lancer le client lourd java
* Lancer le projet java situé dans le dossier *Client*

### En cas de problème de sécurité lors du lancement des serveurs
* Executer les fichiers .exe en tant Administrateur
