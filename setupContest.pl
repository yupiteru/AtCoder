use strict;
use warnings;
use WWW::Mechanize;
use Time::HiRes 'sleep';
use utf8;
binmode STDOUT, ':encoding(cp932)';

open my $propertyFh, "setupContestProperty.txt";
my $loginId = <$propertyFh>;
my $password = <$propertyFh>;
$loginId =~ s/[\r\n]+//g;
$password =~ s/[\r\n]+//g;
close $propertyFh;

my $contestId = "notinput";
if(@ARGV > 0) {
  $contestId = $ARGV[0];
}else {
  $contestId = <STDIN>;
}
$contestId =~ s/[\r\n]+//;

$ENV{'PERL_LWP_SSL_VERIFY_HOSTNAME'} = 0;

my $mech = WWW::Mechanize->new( agent => 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0 Waterfox/56.3') ;
$mech->timeout(120);

$mech->get("https://atcoder.jp/login");
$mech->submit_form(
  form_number => 2,
  fields      => {
    username    => $loginId,
    password    => $password,
  }
);

$mech->get("https://atcoder.jp/contests/$contestId");
my $contestTime = "";
if($mech->content() =~ /<time class='fixtime fixtime-full'>\d+-\d+-\d+ (.+?)\+.+<\/time><\/a>/s) {
  $contestTime = $1;
}

print "コンテストID：$contestId\n";
print "開始時刻：$contestTime\n";
print "待機中...\n";
while(1) {
  my ($sec, $min, $hour, $mday, $mon, $year, $wday, $yday, $isdst) = localtime;
  my $nowTime = sprintf("%02d", $hour) . ":" . sprintf("%02d", $min) . ":" . sprintf("%02d", $sec);
  if($nowTime ge $contestTime) {
    last;
  }
  sleep 1;
}
print "開始\n";
sleep 1;

$mech->get("https://atcoder.jp/contests/$contestId/tasks");
my $problemsPage = $mech->content();
$problemsPage =~ s/[\r\n\t ]//g;
my @problemURLs = $problemsPage =~ /<ahref="(\/contests\/${contestId}\/tasks\/${contestId}_[a-zA-Z])">[A-Z]<\/a>/g;

print "問題数：" . (@problemURLs) . "\n";

my $baseFolder = "problems";
system ".\\setup.pl " . chr(64 + @problemURLs);

for(my $j = 0;$j < @problemURLs; ++$j) {
  my $url = $problemURLs[$j];
  if(fork == 0) {
    $mech->get("https://atcoder.jp$url");
    open my $fh, ">$baseFolder/Problem" . chr(65 + $j) . "_TestCase.txt";
    my $problemContent = $mech->content();
    for(my $i = 1; ; ++$i) {
      if($problemContent =~ /<h3>入力例 $i<\/h3><pre>(.*?)<\/pre>/s) {
        my $input = $1;
        $input =~ s/[\r\n]+/\n/g;
        if($problemContent =~ /<h3>出力例 $i<\/h3><pre>(.*?)<\/pre>/s) {
          my $output = $1;
          $output =~ s/[\r\n]+/\n/g;
          print $fh "[case $i]\n";
          print $fh "$input\n";
          print $fh "[output]\n";
          print $fh "$output\n";
        }else {
          print "error : sample num-$i, url-$url\n";
        }
      }else {
        last;
      }
    }
    close $fh;
    exit;
  }
}

while(wait > 0){}
print "END.\n";

