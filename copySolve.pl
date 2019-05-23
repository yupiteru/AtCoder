use strict;
use warnings;

print "copy to:";
my $to = <STDIN>;

my $toBaseFolder = substr $to, 0, 3;
my $toContestFolder = substr $to, 0, 6;
my $toClass = substr $to, 0, 7;

if(! -e $toBaseFolder) {
  mkdir $toBaseFolder;
}
if(! -e "$toBaseFolder\\$toContestFolder") {
  mkdir "$toBaseFolder\\$toContestFolder";
}

open my $fh, "<template\\ABC000A.cs";
my @data;
while(my $line = <$fh>) {
  $line =~ s/class ABC000A/class $toClass/;
  $data[@data] = $line;
}
close $fh;
open $fh, ">$toBaseFolder\\$toContestFolder\\$toClass.cs";
foreach my $line (@data) {
  print $fh $line;
}
close $fh;

open $fh, "<template\\ABC000A_TestCase.txt";
my @dataCase;
while(my $line = <$fh>) {
  $dataCase[@dataCase] = $line;
}
close $fh;
open $fh, ">$toBaseFolder\\$toContestFolder\\${toClass}_TestCase.txt";
foreach my $line (@dataCase) {
  print $fh $line;
}
close $fh;

