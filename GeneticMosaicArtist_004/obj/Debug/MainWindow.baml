   M S B A M L   `   `   `  ����R  NGeneticMosaicArtist_004, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:GeneticMosaicArtist_004GeneticMosaicArtist_004  5   	   (    "GeneticMosaicArtist_004.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:GeneticMosaicArtist_004     5   	     � Title$  
MainWindow��5   	   $
��1050��6   $
��1850��6*   .��5	      � -   5         �Name$ mainGrid��5	      .+�5
   
   �� -   5         ��Name$ triangleCount_slider��5
      $
��Left=�6/   $��
10,115,0,0q�6J   $	��Top=�6^   $	��285��6v    � Maximum$	 500Q�6�    � SmallChange$ 1Q�6�    � Value$ -1Q�6�    � Minimum$ 10Q�6�   ��6�   Z� ��6  Z� ��6  Z� �� 5   
   -   5         ��Name$ start_button��5      $��Start��6'   $
��Left=�67   $��	10,81,0,0q�6R   $	��Top=�6e   $	��285��6}   $��30��6�   $��18(�6�   $
��Bold"�6�   
���   �6�   �� 5   
   -   5         ��Name$ triangs_textBlock��5      $
��Left=�6/   $��
10,139,0,0q�6J   $
��Wrap=�6^   $	��Top=�6r   $��69��6�   $	��284��6�   $��24(�6�   $
��Bold"�6�   	 �� TextAlignment$	 Center=�6�   ���	#FFB3B3FF�� 6�   $�
Use slider��6�   �� 6	  $� after��6  �� 6  �� 6)  $�load bitmap��6-  6C  �� 5   
   -   5        
 ��Name$
 imageOriginal��5      $��0,719,1542,0q�6'   $	��300��6=   $	��300��6I   �� 5   
   -   5        $ load_button��5      $��Load bitmap��6&   $
��Left=�6<   $��	10,10,0,0q�6W   $	��Top=�6j   $	��284��6�   $��30��6�   $��18(�6�   $
��Bold"�6�   
���   �6�   �� 5   
   -   5        $
 imageGen��5      $��300,0,262,19q�6"   �� 5   
   $
��Left=�6   $��70��6-   $��
11,213,0,0q�69    �� TextWrapping$
 Wrap=�6M   $��Max bmp size: 10240x8000��6a   $	��Top=�6�   $	��284��6�    J� RenderTransformOrigin$ 0.829,-10.457&�6�   $��24(�6�   $
��Bold"�6�    �� TextAlignment$ Center=�6  ���	#FFFFBDBD �� TextDecorations 6�   Z� 6  �� 5   
   -   5        $ save_button��5      $��Save generated bitmap��6&   $
��Left=�6F   $��	10,45,0,0q�6a   $	��Top=�6t   $	��285��6�   $��30��6�   $��18(�6�   $
��Bold"�6�   
���   �6�   5      5      