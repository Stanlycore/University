   M S B A M L   `   `   `  ����  Sha512Cracker$    Sha512CrackerWpfApp.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$-  'Взломщик паролей SHA-512��5   	   $	��650��6*   $	��800��67   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $��* �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ HashesTextBox��5      $��0��6)    �� AcceptsReturn$
 True��66    �� 
AcceptsTab$
 True��6K   $	��100��6]   " �� VerticalScrollBarVisibility$
 Auto=�6j   $��0,0,0,10q�6�   �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5      �� $/��)Количество потоков (1–��6   $��0,0,5,0q�66   �� 5      $��12��6   $��0,0,5,0q�6"   �� 5      $��):��6   $��0,0,5,0q�6"   �� 5      -   5        $ ThreadCountTextBox��5      $��50��62   $��1��6=   $��0,0,10,0q�6F   5      �� 5   
   -   5         ��Name$ ThreadProgressListBox��5      $��2��61   $��0,0,0,10q�6>   �� 5   
   $��3��6   $ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5      �� -   5         ��Name$ TimerTextBlock��5      $-��'Прошедшее время: 00:00:00��60   $��0,0,10,0q�6Q   �� 5       -   5        $ FoundCountTextBlock��5       $��Найдено: 0/0��65   5!      �� 5$   
   $��4��6   $ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5%      �� -   5        	 ��Name$	 StartButton��5%      $��Начать��6*   $	��100��6;   $��0,0,10,0q�6a   �� 5&      -   5        $	 PauseResumeButton��5&      $��
Пауза��60   $	��100��6@   ${�False��6l   $��0,0,10,0q�6~   �� 5'      -	   5        $	 ResetButton��5'      $��
Сброс��6*   $	��100��6:   6F   5(      �� 5+   
   -
   5        $ ResultsTextBox��5+      $��5��6*   
 �� 
IsReadOnly$

 True��67   $	��150��6I   $
 Auto=�6V   5,      5-      