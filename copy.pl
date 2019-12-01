use strict;
use warnings;
use Win32::Clipboard;

opendir my $dh, "lib";
my %library;
my %classToFilename;
my %filenameToUsedLib;
while(my $libfile = readdir $dh) {
  if($libfile =~ /\.cs$/) {
    open my $libfh, "<lib\\$libfile";
    my $libstr = "";
    my $copyStarted = 0;
    while(my $line = <$libfh>) {
      if($copyStarted == 1) {
        if($line =~ m|////end|) {
          last;
        }
        if($line =~ /(class|struct) LIB_([A-Za-z0-9_]+)/) {
          $classToFilename{$2} = $libfile;
        }
        if($line =~ /LIB_([A-Za-z0-9_]+)/) {
          ${$filenameToUsedLib{$libfile}}[@{$filenameToUsedLib{$libfile}}] = $1;
        }
        $libstr .= $line;
      }elsif($line =~ m|////start|) {
        $copyStarted = 1;
      }
    }
    close $libfh;
    $library{$libfile} = $libstr;
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
    my $libFileName = $classToFilename{$1};
    $usedLib{$libFileName} = 1;
  }
}
close $fh;

# todo “ª‚Ì—Ç‚¢•û–@‚Å‚â‚é
my $changed = 1;
while($changed == 1) {
  $changed = 0;
  foreach my $item (keys(%usedLib)) {
    foreach my $item2 (@{$filenameToUsedLib{$item}}) {
      if(! defined $usedLib{$classToFilename{$item2}}) {
        $changed = 1;
        $usedLib{$classToFilename{$item2}} = 1;
      }
    }
  }
}

$str .= "namespace Library {\r\n";
foreach my $item (keys(%usedLib)) {
  $str .= $library{$item};
}
$str .= "}\r\n";

Win32::Clipboard()->Set($str);

