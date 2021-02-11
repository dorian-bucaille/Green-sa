# Green'Sa
Green'Sa est une application mobile ayant pour but d'analyser les performances des golfeurs afin de les conseiller dans leurs plans de jeu.

# Avancement du projet
**Ce qui est déjà fait**
- Application globalement fonctionnelle sur Android
- Historique de partie éditable
- Partage de parcours
- Ajout de golfs

**Ce qu'il reste à faire**
- Réduction de la consommation de batterie
- Interface web pour ajouter des golfs/plan de jeu via un PC
- Corriger erreurs

# Installation et configuration du projet

## 0. Prérequis
- Avoir une machine sous Windows (Visual Studio 2019 n'est pas disponible sur Linux/Mac)
- Avoir un compte GitHub (pour cloner ce repository dans le vôtre)

## 1. Cloner le repository sur votre GitHub

### 1.1 Connectez-vous sur GitHub et cliquez sur 'Import repository' dans le menu déroulant en haut à droite
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/clone_repo_github1.jpg?raw=true" alt="'Import repository' sur GitHub">
</p>

### 1.2 Indiquez l'URL de ce repository et choisissez un nom pour le vôtre
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/clone_repo_github2.jpg?raw=true" alt="Importation du repository sur GitHub">
</p>

## 2. Installer Visual Studio

### 2.1 Téléchargez la dernière version de __Visual Studio 2019 Community__ sur le site officiel de Microsoft : https://visualstudio.microsoft.com/downloads/
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/download_visualstudio.jpg?raw=true" alt="Téléchargement de Visual Studio 2019 Community">
</p>

### 2.2 Ouvrez le fichier 'vs_Community.exe' que vous venez de télécharger
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/install_visualstudio1.jpg?raw=true" alt="Fichier 'vs_Community.exe'">
</p>

### 2.3 Cliquez sur 'Continue'
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/install_visualstudio2.jpg?raw=true" alt="Clic sur 'Continue'">
</p>

### 2.4 Sélectionnez le workload 'Mobile development with .NET' et cliquez sur 'Install'
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/install_visualstudio3.jpg?raw=true" alt="Sélection du workload">
</p>

## 3. Cloner le repository sur Visual Studio

### 3.1 Après l'ouverture de Visual Studio 2019, cliquez sur 'Clone a repository'
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/clone_repo1.jpg?raw=true" alt="Clic sur 'Clone a repository'">
</p>

### 3.2 Indiquez l'URL de votre repository GitHub et cliquez sur 'Clone'
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/clone_repo2.jpg?raw=true" alt="Clonage du repository GitHub">
</p>

## 4. Configurer Visual Studio

### 4.1 Si cette fenêtre s'affiche, laissez tout coché et cliquez sur 'Install'
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_visualstudio1.jpg?raw=true" alt="Auto install">
</p>

### 4.2 Si ce message s'affiche, cliquez sur 'Install' sur le message puis dans la pop-up
<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_visualstudio2.jpg?raw=true" alt="Auto install message">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_visualstudio3.jpg?raw=true" alt="Auto install pop-up">
</p>

### 4.3 Passer le projet sous Android 10.0

#### 4.3.1 Installer les SDK correspondantes en cliquant sur 'Tools', 'Android' puis 'Android SDK Manager...'. En bas à droite, cliquez sur l'engrenage puis 'Repository', 'Full List (Unsupported)'. Ensuite, cochez 'Sources for Android 29' et 'Google Play Intel x86 Atom System Image'. Cliquez sur 'Accept'.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_android.jpg?raw=true" alt="Installer les SDK 1">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_android2.jpg?raw=true" alt="Installer les SDK 2">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_android3.jpg?raw=true" alt="Installer les SDK 3">
</p>

#### 4.3.2 Changer les propriétés du projet en double-cliquant sur 'Properties' dans le menu déroulant de 'GreenSa.Android'. Dans 'Application', sélectionnez 'Android 10.0 (Q)' et cliquez sur 'Yes'. Dans 'Android Manifest', mettez 'Android 4.1 (API Level 16 - Jelly Bean)' pour 'Minimum Android version'. Dans 'Android Options', mettez 'Linking' à 'None'.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_properties.jpg?raw=true" alt="Changer les propriétés 1">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_properties2.jpg?raw=true" alt="Changer les propriétés Application">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_properties3.jpg?raw=true" alt="Changer les propriétés Android Manifest">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_properties4.jpg?raw=true" alt="Changer les propriétés Android Options">
</p>

___Sauvegardez les propriétés (ctrl+s).___

#### 4.3.3 Si ce message apparaît, cliquez sur 'Update Xamarin.Forms Packages'.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/setup_formsupdate.jpg?raw=true" alt="Bandeau Forms">
</p>

### 4.4 Mettre à jour les packages NuGet

#### 4.4.1 Cliquez-droit sur 'GreenSa.Android' puis 'Manage NuGet Packages...'. Vérifiez ici que les plugins 'Xamarin.Forms' et 'Xamarin.Forms.Maps' soient installés et à jour. Si ce n'est pas le cas, faites-le.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/nuget.jpg?raw=true" alt="Ouvrir NuGet">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/nuget2.jpg?raw=true" alt="Check versions à jour NuGet">
</p>

#### 4.4.2 Si des Warnings indiquent qu'il manque des paquets, il faut les réinstaller en utilisant le manager de packages NuGet (mettre le nom du/des package(s) manquant(s) dans la catégorie 'Browse', puis installer)

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/nuget3.jpg?raw=true" alt="Installer un package NuGet">
</p>

### 4.5 Derniers ajustements...

#### 4.5.1 Relancez Visual Studio, puis essayez un build en cliquant sur 'Build' et 'Build Solution'. Si aucune erreur n'apparaît, bravo, vous avez build l'application avec succès !

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/build.jpg?raw=true" alt="Tentative de build">
</p>

#### 4.5.2 Sinon, normalement vous n'avez que des erreurs de références ambiguës. Dans ce cas, pour chaque erreur, double-cliquez dessus pour afficher le fichier posant problème et ajoutez le chemin d'un des deux packages proposés dans l'erreur. Au prochain build, si l'erreur est encore présente alors il faut mettre le chemin de l'autre package.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/ambiguous.jpg?raw=true" alt="Erreur de références">
</p>

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/ambiguous2.jpg?raw=true" alt="Erreur de références : fix">
</p>

#### 4.5.3 Si une erreur 'java.exe' apparaît lors du build, installez Java (https://java.com/en/download/) et le JDK (https://www.oracle.com/java/technologies/javase-downloads.html), puis relancez Visual Studio. Enfin, cliquez sur 'Build', 'Clean Solution' puis re-buildez le projet. L'erreur a normalement disparu.

<p align="center">
  <img src="https://github.com/dorian-bucaille/Green-sa/blob/Fusion/setup_screenshots/error_javaexe.jpg?raw=true" alt="Erreur java.exe">
</p>

### 4.6 (Optionnel) Installez WHPX pour booster les performances de l'émulateur Android. Tutoriel ici : https://developer.android.com/studio/run/emulator-acceleration#vm-windows-whpx

# Télécharger Green'Sa

Une fois le fichier téléchargé, il suffit de l'ouvrir pour lancer le processus d'installation.

**Green'Sa pour Android**
Lien de téléchargement : [com.insaRennes.GreenSa.apk](https://drive.google.com/open?id=1--RuDBP6sxGtZDp6IJAEohftJb-Fe5W0)

**Green'Sa pour iOS**
Non disponible pour le moment.
Un déploiement sur iOS n'est pas envisageable actuellement en l'absence d'un compte développeur Apple.
