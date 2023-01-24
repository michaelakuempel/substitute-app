Anleitung zum Starten der Smartphone-Anwendung auf dem Smartphone durch Generierung einer .apk-Datei
- Es handelt sich um eine Rezeptapp auf Basis von zwei Ontologien: eine Rezeptontologie (von Andreas Keil) und eine Ersatzprodukt-Ontologie (von mir) -

1. GraphDB starten
2. In ein Repository die Ersatzprodukt-Ontologgie (substitutes.owl) und Rezeptontologie (recipe_on_with_mealtypes.owl) aus dem Ordner "Ontologien" hinzufügen (Upload RDF files), baseURI (http://purl.org/NonFoodKG/substitutes# und http://purl.org/NonFoodKG/1-mio-recipes#) für Ontologien angeben und importieren
3. Unity-Projekt über Unity Hub öffnen
4. Alle Szenen in "Scenes In Build" hinzufügen, als Plattform "Android" auswählen (Switch Plattform), Szenen in korrekter Reihenfolge zu "Scenes In Build" hinzufügen (0 CategoryListScreen, 1 RecipeListScreen, 2 RecipeScreen)
5. Wenn Android noch nicht installiert, dann fehlende Androidmodule installieren und Unity neu starten
6. In Unity in der "Game View" sicherstellen, dass die Auflösung korrekt eingestellt ist (1080x2400) und in diesem Zuge die UI-Elemente korrekt angezeigt werden
7. Die .cs-Dateien des Unity-Projektes im \SubstituteApp\Assets\Scripts Ordner in einer IDE oder einem Editor öffnen (CategoryData.cs, RecipeData.cs, RecipeListData.cs, RecipeUIData.cs, SearchData.cs)
8. In allen .cs-Dateien nach "http://localhost:7200/repositories/substitute-app?query=" suchen
9. "localhost" ersetzen mit der IP-Adresse des Geräts auf dem GraphDB mit den Ontologien in einem Repository gestartet ist
10. "substitute-app" ersetzen mit erstelltem Repository-Namen in GraphDB
11. In Unity auf "File" und "Build Settings" klicken
12. Klicke auf "Build"
13. Fertige .apk aufs Handy laden und starten während GraphDB gestartet ist
