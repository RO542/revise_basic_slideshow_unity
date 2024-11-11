using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Assertions;


public class Slideshow : MonoBehaviour {
    public RawImage rawImage;

    private string top_img_folder = @"C:\Users\richa\Downloads\imageNet_images-20241110T235945Z-001\imageNet_images"; //NOTE: change this to be your path to the imageNet_images
    private readonly List<string> all_img_paths = new List<string>();
    private int img_counter = 0;
    private IEnumerator coroutine;
    private float img_interval = 2.0f; // customizable time period that a single image stays on the screen
    private bool random_img_order = true; // variable to control whether images are shown in random order or not
    
    void Start() {
        Debug.Log("Loading all image paths.... ");
        load_all_image_paths();
        StartCoroutine(changeImageCoroutine());
    }
    

    // Instead of using update which runs every frame we are using a coroutine which is a pausable function
    // conveniently unity lets us pause it with a set time interval effectively making a slideshow 
    //void Update() {
    //    //Debug.Log("Updating");
    //}
    

    // loads all images in directory img_folder, non recursive and assumes that there is a folder/folder/many_images structure for now
    public void load_all_image_paths() {
        foreach (var img_dir in Directory.EnumerateDirectories(top_img_folder)) {
            foreach (var img_filepath in Directory.EnumerateFiles(img_dir)) {
                all_img_paths.Add(img_filepath);
                Assert.IsTrue(
                    File.Exists(img_filepath),
                    "A filepath generated in load_all_image_paths() doesn't exist, check the path declaration above."
                );
                //Debug.Log("Loaded image file, path:  " + img_filepath);
            }
        }
    }
   
    // this functions sets the child raw image of the canvas called raw image to be a texture created from the img_filepath
    // there is only a single raw image on the canvas so multiple calls to this function replace the currently shown texture/image to make the slideshow
    void setImageFromPath(string img_filepath) { 
        // load image as byte buffer
        byte[] fileData = File.ReadAllBytes(img_filepath);

        // make a texture from the image and apply it to our raw_image in the canvas
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData)) {
            rawImage.texture = texture;
            rawImage.SetNativeSize();
        } else { 
            Debug.LogError("Unable to load file data for image");
        }
    }

    IEnumerator changeImageCoroutine() {
        while (true) {
            if (random_img_order) {
                setImageFromPath(all_img_paths[UnityEngine.Random.Range(0, all_img_paths.Count - 1)]);
            } else {
                setImageFromPath(all_img_paths[img_counter++]);
                img_counter %= all_img_paths.Count; // wraparound to index 0 if needed
            }
            yield return new WaitForSeconds(img_interval);
        }
    }
}
