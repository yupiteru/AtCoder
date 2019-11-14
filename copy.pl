use strict;
use warnings;
use Win32::Clipboard;

opendir my $dh, "lib";
my %library;
while(my $libfile = readdir $dh) {
  if($libfile =~ /\.cs$/) {
    open my $libfh, "<lib\\$libfile";
    my $libstr = "";
    my $libname = "";
    my $copyStarted = 0;
    while(my $line = <$libfh>) {
      if($copyStarted == 1) {
        if($line =~ m|////end|) {
          last;
        }
        if($line =~ /class LIB_([A-Za-z0-9_]+)/) {
          $libname = $1;
        }
        $libstr .= $line;
      }elsif($line =~ m|////start|) {
        $copyStarted = 1;
      }
    }
    close $libfh;
    $library{$libname} = $libstr;
  }
}
closedir $dh;

my $filename = $ARGV[0];
open my $fh, "<$filename";
my $str = "";
my %usedLib;
while(my $line = <$fh>) {
  $str .= $line;
  while($line =~ /LIB_([A-Za-z0-9_]+)/g) {
    $usedLib{$1} = 1;
  }
}
close $fh;

$str .= "namespace Library {\r\n";
foreach my $item (keys(%usedLib)) {
  $str .= $library{$item};
}
$str .= "}\r\n";

Win32::Clipboard()->Set($str);

