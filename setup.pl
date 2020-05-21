use strict;
use warnings;
use File::Path 'remove_tree';

my $limit = uc <STDIN>;
$limit =~ s/[\r\n]+//;
if(length $limit > 1 or !$limit =~ /[A-Z]/) {
  $limit = "Z";
}
my $baseFolder = "problems";

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
