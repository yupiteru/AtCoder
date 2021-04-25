use strict;
use warnings;
use File::Path 'remove_tree';

my $limit = 'A';
if(@ARGV > 0) {
  $limit = $ARGV[0];
}else {
  $limit = uc <STDIN>;
}

$limit =~ s/[\r\n]+//;
if($limit =~ /[^0-9]/) {
  if(length $limit > 1 or $limit =~ /[^A-Z]/) {
    $limit = "Z";
  }
}else {
  if($limit < 1) {
    $limit = 1;
  }
  if($limit > 999) {
    $limit = 999;
  }
}
my $baseFolder = "problems";

if(! -e $baseFolder) {
  mkdir $baseFolder;
}

opendir my $dh, "$baseFolder";
while(my $file = readdir $dh) {
  if($file =~ /.*\.cs|.*\.txt/) {
    unlink "$baseFolder\\$file";
  }
}
closedir $dh;


open my $fh, "<template\\template.cs";
my @data;
while(my $line = <$fh>) {
  $data[@data] = $line;
}
close $fh;

open $fh, "<template\\template_TestCase.txt";
my @dataCase;
while(my $line = <$fh>) {
  $dataCase[@dataCase] = $line;
}
close $fh;

if($limit =~ /[A-Z]/) {
  for(my $i = 'A';length $i < 2 and $i le $limit; $i++) {
    open $fh, ">$baseFolder\\Problem$i.cs";
    foreach my $line (@data) {
      if($line =~ /class template/) {
        my $tmp = $line;
        $tmp =~ s/class template/class Problem$i/;
        print $fh $tmp;
      } else {
        print $fh $line;
      }
    }
    close $fh;

    open $fh, ">$baseFolder\\Problem${i}_TestCase.txt";
    foreach my $line (@dataCase) {
      print $fh $line;
    }
    close $fh;
  }
}else {
  for(my $i = 1; $i <= $limit; $i++) {
    my $str = sprintf("%03d", $i);
    open $fh, ">$baseFolder\\Problem$str.cs";
    foreach my $line (@data) {
      if($line =~ /class template/) {
        my $tmp = $line;
        $tmp =~ s/class template/class Problem$str/;
        print $fh $tmp;
      } else {
        print $fh $line;
      }
    }
    close $fh;

    open $fh, ">$baseFolder\\Problem${str}_TestCase.txt";
    foreach my $line (@dataCase) {
      print $fh $line;
    }
    close $fh;
  }
}
