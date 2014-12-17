using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace XMLstuff {
    public class Program {

        public class PetStore {

            public PetStore() {
                PetStoreName = "";
                PetStoreCity = "";
                PetStoreZip = null;
                PetList = new List<Pet>();
            }
            public List<Pet> PetList { get; set; } // note that you get or set the whole list -- this makes all the list methods available
            public String PetStoreName { get; set; }
            public String PetStoreCity { get; set; }
            public int? PetStoreZip { get; set; }
        }

        public class PetListWrapper {

            public PetListWrapper() {
                PetList = new List<Pet>();
            }
            public List<Pet> PetList { get; set; }
        }

        public class Pet {
            public Pet() { // the XmlSerializer requires an empty constructor here
                PetType = "";
                PetName = "";
                Breed = "";
                Weight = null;
            }
            public Pet(String pettype, String name, String breed, int weight) {
                PetType = pettype;
                PetName = name;
                Breed = breed;
                Weight = weight;
            }
            public String PetType { get; set; } // the XmlSerializer requires public setters for all properties unless flagged as an ignored property
            //[XmlIgnore]                         // this flag causes PetName to be ignored by the Xml Serializer
            public String PetName { get; set; }
            public String Breed { get; set; }
            public int? Weight { get; set; }
        }

        public static void Main(String[] args) {

            ArrayList arrlist = new ArrayList(); // non-generic ArrayList -- the SERIALIZER WILL NOT WORK

            List<Pet> myPetList1 = new List<Pet>(); // just a list

            PetListWrapper myPetList2 = new PetListWrapper(); // a list inside a wrapper class

            PetStore myPetStore = new PetStore(); // a list and some other items inside a wrappr class

            myPetStore.PetStoreName = "Polly's Pets";
            myPetStore.PetStoreCity = "St. Louis";
            myPetStore.PetStoreZip = 63011;

            String[] ptypes = new String[] { "Dog", "Dog", "Dog", "Dog", "Cat"};
            String[] names = new String[] { "Rover", "Spot", "Lassie", "Fido", "Fluffy"};
            String[] breeds = new String[] { "Beagle", "Dalmation", "Collie", "Boxer", "Calico"};
            int[] weights = new int[] { 30, 50, 60, 60, 12 };

            for (int i = 0 ; i < names.Length ; i++) {
                Pet p = new Pet(ptypes[i], names[i], breeds[i], weights[i]);
                myPetList1.Add(p);
                myPetList2.PetList.Add(p);
                myPetStore.PetList.Add(p);
                arrlist.Add(p);
            }

            Console.WriteLine("\n\nAn individual object:\n");
            Pet p1 = new Pet("fish", "Wanda", "Goldfish",0);

            //new XmlSerializer(typeof(Pet)).Serialize(Console.Out, p1); // this works
            var xmlser = new XmlSerializer(typeof(Pet)); // note that this can be done in two conventional steps - create object
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add("davejjj", "https://github.com/davejjj"); // create some customized xmlns settings
            xmlser.Serialize(Console.Out, p1, xmlns); // and then use the object

            Console.WriteLine("\n\nA typed list of objects in a wrapper class:\n"); // the "encoding=IBM437" is only seen in the Console, not in the file
            new XmlSerializer(typeof(PetListWrapper)).Serialize(Console.Out, myPetList2, xmlns); // this works
            
            Console.WriteLine("\n\nA typed list of objects:\n");
            new XmlSerializer(typeof(List<Pet>)).Serialize(Console.Out, myPetList1, xmlns); // this works

            //Console.WriteLine("\n\nA NON-TYPED COLLECTION such as an ordinary ArrayList:\n");
            //new XmlSerializer(typeof(ArrayList)).Serialize(Console.Out, arrlist, xmlns); // this DOES NOT work

            String fpath = @"C:\Temp\Test01\test.xml";
            Console.WriteLine("\n\nSave a list of objects to an xml file: {0}", fpath);
            FileStream fs = new FileStream(fpath, FileMode.Create); // create or overwrite file
            new XmlSerializer(typeof(List<Pet>)).Serialize(fs, myPetList1, xmlns); // this works
            fs.Flush();
            fs.Close();
            Console.WriteLine("Filestream closed");

            Console.WriteLine("\n\nA petstore:\n");
            new XmlSerializer(typeof(PetStore)).Serialize(Console.Out, myPetStore, xmlns); // this works

            Console.WriteLine("\n\nPress a key...");
            Console.ReadKey();

            Console.WriteLine("\n\nNow Read the file back... Press a key...");
            Console.ReadKey();

            Console.WriteLine("\n\nRead an xml file to create a list of objects: {0}", fpath);
            XmlSerializer rxfstrm = new XmlSerializer(typeof(List<Pet>));
            FileStream rfs = new FileStream(fpath, FileMode.Open);
            List<Pet> newpetlist = (List<Pet>)rxfstrm.Deserialize(rfs);
            foreach(Pet x in newpetlist){
                Console.WriteLine("\nPettype:{0} \nPetname:{1} \nBreed:{2} \nWeight:{3}", x.PetType, x.PetName, x.Breed, x.Weight);
            }
            rfs.Flush();
            rfs.Dispose();

            Console.WriteLine("\n\nPress a key...");
            Console.ReadKey();

        } // end of Main
    } // end of class
} // end of namespace
