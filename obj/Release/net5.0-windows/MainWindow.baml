   M S B A M L   `   `   `  ���� D  @EcoleData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:EcoleData	EcoleData      EcoleData.MainWindow   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Y UOxyPlot.Wpf.Shared, Version=2.1.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0R NOxyPlot.Wpf, Version=2.1.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   %localclr-namespace:EcoleData   -     �Name$  layoutWindow�� � Title$* $EcoleDATA - TPI Morgane Lebre v1.0.0��$	��800��$
��1280�� � WindowState$ 	Maximized=�$	��600��$
��1000�� � 	Resources    
FilterIcon �� 	UriSource  CalendarIcon  ClearFiltersIcon  LocationIcon  
ValuesIcon  SchoolBuildingIcon  
StairsIcon  	GraphIcon  
FolderIcon 	 FullLine 
 
DashedLine  MainLogo��%�  &	        &	 -     &	 \     &	 �     &	 �     &	 �     &	 (    &	 X    &	 �    &		 �    &	
 �    &	 �    �� $' !/Media/icons/icons8-filter-16.png?��� $) #/Media/icons/icons8-calendar-16.png?��� $. (/Media/icons/icons8-clear-filters-16.png?��� $) #/Media/icons/icons8-location-16.png?��� $- '/Media/icons/icons8-plus-moins-2-16.png?��� $0 */Media/icons/icons8-school-building-16.png?��� $* $/Media/icons/icons8-stairs-up-16.png?��� $' !/Media/icons/icons8-stocks-16.png?��� $' !/Media/icons/icons8-folder-48.png?��� $ /Media/fullLine.png?��� $ /Media/dashedLine.png?��� $ /Media/mainLogo.png?� � DataContext    EcoleData.OxyViewModel  .���  � ColumnDefinitions �� $	��330 ��� $��* � � RowDefinitions �� $	��100 ��� $��* ��� $��50 �.+��� $��0��$��0��$��2�� �� Orientation$ 
Horizontal=�.+��� #��[ $��10, 10, 20, 10q��� $��50(� 
 Roboto#��[ $
��Bold"�$��Center=�
ÉcoleDATA]� $��1��$��0��$��10q�.+��� -   	 ��Name$	 loadMessageTB��$��50, 20, 0, 20q�$��Center=�#��[ $��20(�%#Chargement des données en cours...� $��Right=�$��0,0,5,0q� �� $��* ��� $��* �.+��� $ 
Horizontal=�.+��� $��0��$��Center=�#��[ $��31��$��28���� -   $	 folderNameTB��$��0��$��Center=�$��7,0,0,0q�$��14(�#��[ 
�������" [chemin du fichier des dossiers]�� -   $��1��$��Right=�$��30��$	��150��#��[   BtnStyle#��[ .��Choisir dossier...�� $��1��$��0��$��
10,10,5,10q�#��[ $��2q�
���   ����� $ 
Horizontal=�.+��� #��[  �� $��	UltraBold"�$��20(�$��7,0,0,0q�	FILTRES.���  �� $��30 ��� $��* ��� $��50 �.+�]� $��0��.+��� -   
 ��Name$
 resetFiltersBtn��$��Right=�$��0,0,10,0q�
������ $��0q�$y�Visible=�.���� $ 
Horizontal=�.+��� $��16��#��[ �� $��7,0,7,0q�$��Center=�Réinitialiser les filtres�� $��1�� J� RenderTransformOrigin$ 0.5,0.5&�$��5,0,10,0q�z�V� .~��� ��  �� AngleY$ 0Q��� T�  T� Y$ 1.495Q�.���� $ Vertical=�.+��� $	��100��  FilterGroupBox#��[ ���� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��14(�$��5,0,0,0q�ÉCOLE.��� $��0q� �� $
��auto ��� $��* � �� $��* ��� $��* ��� $��* �.+��� -    ��Name$ schoolsComboBox��$��0��$��0��$��2��$��25��$��10q�${�False��$��16(� �� Text$! Sélectionner une école...���� -   $	 schoolTitle��$��1��$��1��$��14(�$
��Bold"�$��Bottom=� �� TextAlignment$ Center=�$��5q�École de [nom]�� -   $	 schoolCaptorsNb��$��1��$��2��$
��Bold"�$ Center=�
X capteurs� -	    �Name$ 	logoPlace��$
}�True��$��0��$��1��$��2��$��70��$��30, 10, 10, 10q��� $��80��#��[ ���� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��14(�$��5,0,0,0q�ÉTAGE.��� -
   $ 
floorsGrid��$��5,5,5,2q� �� �� �� �� ��  �� �� �� -   $	 floorsFiltersErrorMsg��  ErrorMessageStyle#��[ %#Veuillez cocher au moins un étage.�� $��80��#��[ ���� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��14(�$��5,0,0,0q�EMPLACEMENT.���� $ Vertical=�$��5q�.+�]� .+��� -    ��Name$ salleSensorCB��$��0,5,0,5q�.��Capteurs salles�� #��[	 $��10, 0, 0, 0q�$��69��$��Right=�]� .+��� -   $ couloirSensorCB��$��0,5,0,0q�.��Capteurs couloirs�� #��[
 $��69��$��Right=��� -   $	 locationsFilterErrorMsg��#��[ *(Veuillez cocher au moins un emplacement.�� $��80��#��[ ���� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��14(�$��5,0,0,0q�	VALEURS.��� $��5q� �� ��  �� �� .+��� -   $ temperatureCB��$��0��$��0��  CheckBoxStyle#��[ .��Température�� -   $ 
dewPointCB��$��0��$��1��#��[ .��Point de rosée�� -   $ 
humidityCB��$��1��$��0��#��[ .��	Humidité�� -   $	 valuesFiltersErrorMsg��#��[ &$Veuillez cocher au moins une valeur.�� #��[ ���� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��14(�$��5,0,0,0q�PLAGE DE DATES.���� $ 
Horizontal=�$��5q�.+��� $��Center=�du(  "System.Windows.Controls.DatePicker  -     Name$ startDatePicker��  styleCalendar   CalendarStyle# [ $��Center=�$	��100��   SelectedDate 
01-01-1970$��5q��� $��Center=�au  -   $ endDatePicker��# [ $��Center=�$	��100�� 
01-01-1970$��5q��� -   $	 datesFilterErrorMsg��#��[ )'Une erreur est survenue avec les dates.�� -   $
 applyFiltersBtn��#��[ $��20,10,20,10q�$��2��$��Appliquer les filtres���� $��1��$��1��$��
5,10,10,10q�#��[ $��2q�
���   ����� $ 
Horizontal=�.+��� #��[ �� $��	UltraBold"�$��20(�$��7,0,0,0q�	GRAPHIQUE.��� -   $ 
graphPlace��.+��� -   $	 	noDataMsg��$ Center=�$��Center=�
�������Pas de données à afficher.�� $��2��$��2��$ Vertical=�$��10q�.+��� $ Right=�
�������$��0,0,20,0q�'%ÉcoleDATA - TPI Morgane Lebre v1.0.0