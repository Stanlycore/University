   M S B A M L   `   `   `  ����   Sha512Cracker$    Sha512CrackerWpfApp.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$-  'Взломщик паролей SHA-512��$	��650��$	��800��.��� $��10q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ HashesTextBox��$��0�� �� AcceptsReturn$
 True�� �� 
AcceptsTab$
 True��$	��100��" �� VerticalScrollBarVisibility$
 Auto=�$��0,0,0,10q��� $��1�� �� Orientation$ 
Horizontal=�$��0,0,0,10q�.+��� $*��$Количество потоков:��$��0,0,5,0q��� -   $ ThreadCountTextBox��$��50��$��1��$��0,0,10,0q��� -    ��Name$ ThreadProgressListBox��$��2��$��0,0,0,10q��� $��3��$ 
Horizontal=�$��0,0,0,10q�.+��� -    ��Name$ TimerTextBlock��$-��'Прошедшее время: 00:00:00��$��0,0,10,0q��� -   $ FoundCountTextBlock��$��Найдено: 0/0���� $��4��$ 
Horizontal=�$��0,0,0,10q�.+��� -   	 ��Name$	 StartButton��$��Начать��$	��100��$��0,0,10,0q��� -   $	 PauseResumeButton��$��
Пауза��$	��100��${�False��$��0,0,10,0q��� -	   $	 ResetButton��$��
Сброс��$	��100���� -
   $ ResultsTextBox��$��5��
 �� 
IsReadOnly$

 True��$	��150��$
 Auto=�