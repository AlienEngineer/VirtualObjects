<?php
    include 'libs/Parsedown.php';
    $Parsedown = new Parsedown();
    echo $Parsedown->fromFile('markdown/home.md');
?>