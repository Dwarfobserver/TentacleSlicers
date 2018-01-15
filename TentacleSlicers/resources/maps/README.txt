/---------------------\
 COMMENT CREER DES MAP
\---------------------/

Le dossier "terrains" doit contenir tous les murs, d'une hitbox 92x92 px, qui
seront utilisés pour les map, au format png, avec comme nom un numéro
consécutif d'un numéro s'y trouvant déjà (allant de 1 à 9).

Un fichier map, avec le même type de nom et l'extension .map, décrit dans sa
première ligne la map avec d'abord son nom, puis le nombre de colonnes et enfin
de lignes.

Ensuite, une matrice de la taille indiquée dans la première ligne indique ce
qui se trouve à chaque case :
  x     - case vide.
  p     - indique le point d'apparition d'un joueur. Il en faut 2 au minimum.
  g     - indique un point d'apaprtion des mobs. Il en faut plusieurs.
  f     - indique où apapraît le boss. Il n'en faut qu'un.
  <num> - place un mur avec le sprite ayant le même numéro dans le dossier 
          "terrains". Si le numéro est 0, alors le mur est invisible.
